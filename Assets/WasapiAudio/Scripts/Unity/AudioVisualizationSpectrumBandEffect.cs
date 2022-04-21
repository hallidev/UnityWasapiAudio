using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public abstract class AudioVisualizationSpectrumBandEffect : MonoBehaviour
    {
        private float[] _spectrumData;

        // Inspector properties
        public int SpectrumBand;

        public void SetSpectrumData(float[] spectrumData)
        {
            _spectrumData = spectrumData;
        }

        protected float[] GetSpectrumData()
        {
            return _spectrumData;
        }

        protected float GetSpectrumBandData()
        {
            if (_spectrumData == null)
            {
                return 0.0f;
            }

            return GetSpectrumData()[SpectrumBand];
        }
    }
}
