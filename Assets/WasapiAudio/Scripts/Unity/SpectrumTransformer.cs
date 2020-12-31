using System;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [Serializable]
    public abstract class SpectrumTransformer
    {
        private float[] _output;

        public float[] Transform(float[] spectrumData)
        {
            if (_output == null || _output.Length != spectrumData.Length)
            {
                _output = new float[spectrumData.Length];
            }

            PerformTransform(spectrumData, _output);

            return _output;
        }

        protected abstract void PerformTransform(float[] input, float[] output);
    }
}
