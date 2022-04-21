using System;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class SpectrumDescriptor
    {
        public string Key { get; }
        public int SpectrumSize { get; }
        public ScalingStrategy ScalingStrategy { get; }
        public WindowFunctionType WindowFunctionType { get; }
        public int MinFrequency { get; }
        public int MaxFrequency { get; }
        public Action<float[]> ReceiveAudio { get; }

        public SpectrumDescriptor(string key, int spectrumSize, ScalingStrategy scalingStrategy,
            WindowFunctionType windowFunctionType, int minFrequency, int maxFrequency,
            Action<float[]> receiveAudio)
        {
            Key = key;
            SpectrumSize = spectrumSize;
            ScalingStrategy = scalingStrategy;
            WindowFunctionType = windowFunctionType;
            MinFrequency = minFrequency;
            MaxFrequency = maxFrequency;
            ReceiveAudio = receiveAudio;
        }
    }
}
