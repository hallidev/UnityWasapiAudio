namespace Assets.WasapiAudio.Scripts.Unity
{
    public abstract class AudioVisualizationSpectrumBandEffect : AudioVisualizationEffect
    {
        // Inspector properties
        public int SpectrumBand;

        protected float GetSpectrumBandData()
        {
            return GetSpectrumData()[SpectrumBand];
        }
    }
}
