using System;

namespace Assets.WasapiAudio.Scripts.Core
{
    public enum WasapiAudioFilterType
    {
        LowPass,
        HighPass,
        BandPass,
    }

    [Serializable]
    public class WasapiAudioFilter
    {
        public WasapiAudioFilterType Type;
        public int Frequency;
    }
}
