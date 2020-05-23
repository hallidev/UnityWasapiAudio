using System;

namespace Assets.WasapiAudio.Scripts.Unity
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
