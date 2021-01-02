using System;
using System.Linq;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [Serializable]
    public abstract class SpectrumTransformer
    {
        private float[] _input;
        private float[] _output;

        public bool Enabled = true;

        protected float InputMin { get; private set; }
        protected float InputMax { get; private set; }
        protected float InputAverage { get; private set; }
        protected float InputRange { get; private set; }

        public float[] Transform(float[] input)
        {
            _input = input;

            if (_output == null || _output.Length != _input.Length)
            {
                _output = new float[_input.Length];
            }

            InputMin = _input.Min();
            InputMax = _input.Max();
            InputAverage = _input.Average();
            InputRange = InputMax - InputMin;

            if (Enabled)
            {
                PerformTransform(_input, ref _output);
            }

            return Enabled ? _output : _input;
        }

        protected abstract void PerformTransform(float[] input, ref float[] output);
    }
}
