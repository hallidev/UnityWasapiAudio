using System;
using System.Collections.Generic;
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
        private readonly WasapiAudioFilter[] _filters;
        private readonly Dictionary<SpectrumReceiver, SpectrumInfo> _spectrumInfos = new();

        private WasapiCapture _wasapiCapture;
        private SoundInSource _soundInSource;
        private IWaveSource _realtimeSource;
        private SingleBlockNotificationStream _singleBlockNotificationStream;
        
        public WasapiAudio(WasapiCaptureType captureType, WasapiAudioFilter[] filters)
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
            _captureType = captureType;
            _filters = filters;
        }

        public void AddReceiver(SpectrumReceiver receiver)
        {
            WindowFunction windowFunction = null;

            switch (receiver.WindowFunctionType)
            {
                case WindowFunctionType.None:
                    windowFunction = WindowFunctions.None;
                    break;
                case WindowFunctionType.Hamming:
                    windowFunction = WindowFunctions.Hamming;
                    break;
                case WindowFunctionType.HammingPeriodic:
                    windowFunction = WindowFunctions.HammingPeriodic;
                    break;
                case WindowFunctionType.Hanning:
                    windowFunction = WindowFunctions.Hanning;
                    break;
                case WindowFunctionType.HanningPeriodic:
                    windowFunction = WindowFunctions.HanningPeriodic;
                    break;
                case WindowFunctionType.BlackmannHarris:
                    windowFunction = WindowFunctions.BlackmannHarris;
                    break;
                default:
                    throw new Exception("Unknown WindowFunctionType");
            }

            var basicSpectrumProvider = new BasicSpectrumProvider(_soundInSource.WaveFormat.Channels, _soundInSource.WaveFormat.SampleRate, CFftSize, windowFunction);

            var lineSpectrum = new LineSpectrum(CFftSize, receiver.MinFrequency, receiver.MaxFrequency)
            {
                SpectrumProvider = basicSpectrumProvider,
                BarCount = receiver.SpectrumSize,
                UseAverage = true,
                IsXLogScale = true,
                ScalingStrategy = receiver.ScalingStrategy
            };

            _spectrumInfos.Add(receiver, new SpectrumInfo(basicSpectrumProvider, lineSpectrum));
        }

        public void StartCapture()
        {
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
                    // Emit events for all of the receivers
                    foreach (var spectrum in _spectrumInfos.Keys)
                    {
                        var spectrumInfo = _spectrumInfos[spectrum];
                        var spectrumData = spectrumInfo.LineSpectrum.GetSpectrumData(MaxAudioValue);

                        if (spectrumData != null)
                        {
                            spectrum.ReceiveAudio?.Invoke(spectrumData);
                        }
                    }
                }
            };

            _singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStream_SingleBlockRead;
        }

        public void StopCapture()
        {
            _singleBlockNotificationStream.SingleBlockRead -= SingleBlockNotificationStream_SingleBlockRead;

            _soundInSource.Dispose();
            _realtimeSource.Dispose();
            _wasapiCapture.Stop();
            _wasapiCapture.Dispose();
        }

        private void SingleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            // Feed all of the spectra
            foreach (var spectrumInfo in _spectrumInfos.Values)
            {
                spectrumInfo.Provider.Add(e.Left, e.Right);
            }
        }

        private class SpectrumInfo
        {
            public BasicSpectrumProvider Provider { get; }
            public LineSpectrum LineSpectrum { get; }

            public SpectrumInfo(BasicSpectrumProvider provider, LineSpectrum lineSpectrum)
            {
                Provider = provider;
                LineSpectrum = lineSpectrum;
            }
        }
    }
}
