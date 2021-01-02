using System;
using Assets.WasapiAudio.Scripts.Unity;
using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class WasapiAudio
    {
        private const FftSize CFftSize = FftSize.Fft4096;
        private const float MaxAudioValue = 1.0f;

        private readonly WasapiCaptureType _captureType;
        private readonly int _spectrumSize;
        private readonly ScalingStrategy _scalingStrategy;
        private readonly int _minFrequency;
        private readonly int _maxFrequency;

        private WasapiCapture _wasapiCapture;
        private SoundInSource _soundInSource;
        private IWaveSource _realtimeSource;
        private BasicSpectrumProvider _basicSpectrumProvider;
        private LineSpectrum _lineSpectrum;
        private SingleBlockNotificationStream _singleBlockNotificationStream;
        private WasapiAudioFilter[] _filters;
        private Action<float[]> _receiveAudio;

        public WasapiAudio(WasapiCaptureType captureType, int spectrumSize, ScalingStrategy scalingStrategy, int minFrequency, int maxFrequency, WasapiAudioFilter[] filters, Action<float[]> receiveAudio)
        {
            _captureType = captureType;
            _spectrumSize = spectrumSize;
            _scalingStrategy = scalingStrategy;
            _minFrequency = minFrequency;
            _maxFrequency = maxFrequency;
            _filters = filters;
            _receiveAudio = receiveAudio;
        }

        public void StartListen()
        {
            switch (_captureType)
            {
                case WasapiCaptureType.Loopback:
                    _wasapiCapture = new WasapiLoopbackCapture();
                    break;
                case WasapiCaptureType.Microphone:
                    MMDevice defaultMicrophone;
                    using (var deviceEnumerator = new MMDeviceEnumerator())
                    {
                        defaultMicrophone = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                    }
                    _wasapiCapture = new WasapiCapture();
                    _wasapiCapture.Device = defaultMicrophone;
                    break;
                default:
                    throw new InvalidOperationException("Unhandled WasapiCaptureType");
            }

            _wasapiCapture.Initialize();

            _soundInSource = new SoundInSource(_wasapiCapture);

            _basicSpectrumProvider = new BasicSpectrumProvider(_soundInSource.WaveFormat.Channels, _soundInSource.WaveFormat.SampleRate, CFftSize);

            _lineSpectrum = new LineSpectrum(CFftSize, _minFrequency, _maxFrequency)
            {
                SpectrumProvider = _basicSpectrumProvider,
                BarCount = _spectrumSize,
                UseAverage = true,
                IsXLogScale = true,
                ScalingStrategy = _scalingStrategy
            };

            _wasapiCapture.Start();

            var sampleSource = _soundInSource.ToSampleSource();

            if (_filters != null && _filters.Length > 0)
            {
                foreach (var filter in _filters)
                {
                    sampleSource = sampleSource.AppendSource(x => new BiQuadFilterSource(x));
                    var biQuadSource = (BiQuadFilterSource) sampleSource;
                    
                    switch (filter.Type)
                    {
                        case WasapiAudioFilterType.LowPass:
                            biQuadSource.Filter = new LowpassFilter(_soundInSource.WaveFormat.SampleRate, filter.Frequency);
                            break;
                        case WasapiAudioFilterType.HighPass:
                            biQuadSource.Filter = new HighpassFilter(_soundInSource.WaveFormat.SampleRate, filter.Frequency);
                            break;
                        case WasapiAudioFilterType.BandPass:
                            biQuadSource.Filter = new BandpassFilter(_soundInSource.WaveFormat.SampleRate, filter.Frequency);
                            break;
                    }
                }
            }

            _singleBlockNotificationStream = new SingleBlockNotificationStream(sampleSource);
            _realtimeSource = _singleBlockNotificationStream.ToWaveSource();

            var buffer = new byte[_realtimeSource.WaveFormat.BytesPerSecond / 2];

            _soundInSource.DataAvailable += (s, ea) =>
            {
                while (_realtimeSource.Read(buffer, 0, buffer.Length) > 0)
                {
                    var spectrumData = _lineSpectrum.GetSpectrumData(MaxAudioValue);

                    if (spectrumData != null)
                    {
                        _receiveAudio?.Invoke(spectrumData);
                    }
                }
            };

            _singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStream_SingleBlockRead;
        }

        public void StopListen()
        {
            _singleBlockNotificationStream.SingleBlockRead -= SingleBlockNotificationStream_SingleBlockRead;

            _soundInSource.Dispose();
            _realtimeSource.Dispose();
            _receiveAudio = null;
            _wasapiCapture.Stop();
            _wasapiCapture.Dispose();
        }

        private void SingleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            _basicSpectrumProvider.Add(e.Left, e.Right);
        }
    }
}
