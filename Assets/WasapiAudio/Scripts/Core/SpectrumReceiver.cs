using System;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class SpectrumReceiver
    {
        public int SpectrumSize { get; }
        public ScalingStrategy ScalingStrategy { get; }
        public WindowFunctionType WindowFunctionType { get; }
        public int MinFrequency { get; }
        public int MaxFrequency { get; }
        public Action<float[]> ReceiveAudio { get; }

        public SpectrumReceiver(int spectrumSize, ScalingStrategy scalingStrategy,
            WindowFunctionType windowFunctionType, int minFrequency, int maxFrequency,
            Action<float[]> receiveAudio)
        {
            SpectrumSize = spectrumSize;
            ScalingStrategy = scalingStrategy;
            WindowFunctionType = windowFunctionType;
            MinFrequency = minFrequency;
            MaxFrequency = maxFrequency;
            ReceiveAudio = receiveAudio;
        }
    }
}
