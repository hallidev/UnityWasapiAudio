using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class WasapiAudio
    {
        private const FftSize CFftSize = FftSize.Fft4096;
        private const float MaxAudioValue = 1.0f;

        private readonly WasapiCaptureType _captureType;
        private readonly int _spectrumSize;
        private readonly ScalingStrategy _scalingStrategy;
        private readonly WindowFunctionType _windowFunctionType;
        private readonly int _minFrequency;
        private readonly int _maxFrequency;

        private WasapiCapture _wasapiCapture;
        private WaveInProvider _waveInProvider;
        private WaveToSampleProvider _waveToSampleProvider;
        private NotifyingSampleProvider _notifyingSampleProvider;
        private BasicSpectrumProvider _basicSpectrumProvider;
        private LineSpectrum _lineSpectrum;
        private WasapiAudioFilter[] _filters;
        private Action<float[]> _receiveAudio;

        public WasapiAudio(WasapiCaptureType captureType, int spectrumSize, ScalingStrategy scalingStrategy, WindowFunctionType windowFunctionType, int minFrequency, int maxFrequency, WasapiAudioFilter[] filters, Action<float[]> receiveAudio)
        {
            _captureType = captureType;
            _spectrumSize = spectrumSize;
            _scalingStrategy = scalingStrategy;
            _windowFunctionType = windowFunctionType;
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
                    MMDeviceEnumerator devices = new MMDeviceEnumerator();
                    var ld = devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    _wasapiCapture = new WasapiLoopbackCapture();
                    break;
                case WasapiCaptureType.Microphone:
                    MMDevice defaultMicrophone;
                    using (var deviceEnumerator = new MMDeviceEnumerator())
                    {
                        defaultMicrophone = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                    }
                    _wasapiCapture = new WasapiCapture(defaultMicrophone);
                    break;
                default:
                    throw new InvalidOperationException("Unhandled WasapiCaptureType");
            }

            _waveInProvider = new WaveInProvider(_wasapiCapture);
            _waveToSampleProvider = new WaveToSampleProvider(_waveInProvider);
            _notifyingSampleProvider = new NotifyingSampleProvider(_waveToSampleProvider);

            WindowFunction windowFunction = null;

            switch (_windowFunctionType)
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

            _basicSpectrumProvider = new BasicSpectrumProvider(_wasapiCapture.WaveFormat.Channels, _wasapiCapture.WaveFormat.SampleRate, CFftSize, windowFunction);

            _lineSpectrum = new LineSpectrum(CFftSize, _minFrequency, _maxFrequency)
            {
                SpectrumProvider = _basicSpectrumProvider,
                BarCount = _spectrumSize,
                UseAverage = true,
                IsXLogScale = true,
                ScalingStrategy = _scalingStrategy
            };

            if (_filters != null && _filters.Length > 0)
            {
                //foreach (var filter in _filters)
                //{
                //    sampleSource = sampleSource.AppendSource(x => new BiQuadFilterSource(x));
                //    var biQuadSource = (BiQuadFilterSource) sampleSource;
                    
                //    switch (filter.Type)
                //    {
                //        case WasapiAudioFilterType.LowPass:
                //            biQuadSource.Filter = new LowpassFilter(_soundInSource.WaveFormat.SampleRate, filter.Frequency);
                //            break;
                //        case WasapiAudioFilterType.HighPass:
                //            biQuadSource.Filter = new HighpassFilter(_soundInSource.WaveFormat.SampleRate, filter.Frequency);
                //            break;
                //        case WasapiAudioFilterType.BandPass:
                //            biQuadSource.Filter = new BandpassFilter(_soundInSource.WaveFormat.SampleRate, filter.Frequency);
                //            break;
                //    }
                //}
            }

            var buffer = new byte[_wasapiCapture.WaveFormat.AverageBytesPerSecond / 2];

            _wasapiCapture.DataAvailable += (s, ea) =>
            {
                while (_waveInProvider.Read(buffer, 0, buffer.Length) > 0)
                {
                    var spectrumData = _lineSpectrum.GetSpectrumData(MaxAudioValue);

                    if (spectrumData != null)
                    {
                        _receiveAudio?.Invoke(spectrumData);
                    }
                }
            };

            _wasapiCapture.StartRecording();

            _notifyingSampleProvider.Sample += NotifyingSampleProvider_Sample;
        }

        public void StopListen()
        {
            _notifyingSampleProvider.Sample -= NotifyingSampleProvider_Sample;

 
            _receiveAudio = null;
            _wasapiCapture.StopRecording();
            _wasapiCapture.Dispose();
        }

        private void NotifyingSampleProvider_Sample(object sender, SampleEventArgs e)
        {
            _basicSpectrumProvider.Add(e.Left, e.Right);
        }
    }
}
