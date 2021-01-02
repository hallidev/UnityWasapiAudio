using System;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity.Transformers
{
    [Serializable]
    public class CurveScaledSpectrumTransformer : SpectrumTransformer
    {
        public AnimationCurve ScaleCurve;

        protected override void PerformTransform(float[] input, ref float[] output)
        {
            if (ScaleCurve == null)
            {
                return;
            }

            var scaleStep = 1.0f / input.Length;

            for (int i = 0; i < input.Length; i++)
            {
                var scaledValue = ScaleCurve.Evaluate(scaleStep * i) * input[i];

                output[i] = scaledValue;
            }
        }
    }
}
