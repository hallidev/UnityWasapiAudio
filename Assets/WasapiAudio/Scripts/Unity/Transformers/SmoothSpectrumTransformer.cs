using System;
using Assets.WasapiAudio.Scripts.Core;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity.Transformers
{
    [Serializable]
    public class SmoothSpectrumTransformer : SpectrumTransformer
    {
        private SpectrumSmoother _smoother;

        [Range(1, 50)]
        public byte Iterations = 1;

        protected override void PerformTransform(float[] input, ref float[] output)
        {
            if (_smoother == null || _smoother.SmoothingIterations != Iterations)
            {
                _smoother = new SpectrumSmoother(input.Length, Iterations);
            }

            output = _smoother.GetSpectrumData(input);
        }
    }
}
