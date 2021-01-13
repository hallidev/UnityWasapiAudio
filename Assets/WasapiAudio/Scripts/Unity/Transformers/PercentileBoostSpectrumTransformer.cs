using System;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity.Transformers
{
    [Serializable]
    public class PercentileBoostSpectrumTransformer : SpectrumTransformer
    {
        private float[] _sortedPercentileArray;
        
        [Range(0.0f, 1.0f)]
        public float Percentile = 0.5f;
        public float BelowPercentileMultiplier = 0.5f;
        public float AbovePercentileMultiplier = 2.0f;

        protected override void PerformTransform(float[] input, ref float[] output)
        {
            if (_sortedPercentileArray == null || _sortedPercentileArray.Length != input.Length)
            {
                _sortedPercentileArray = new float[input.Length];
            }
            
            input.CopyTo(_sortedPercentileArray, 0);
            
            var percentile = GetPercentile(_sortedPercentileArray, Percentile);
            
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < percentile)
                {
                    output[i] = input[i] * BelowPercentileMultiplier;
                }
                else
                {
                    output[i] = input[i] * AbovePercentileMultiplier;
                }
            }
        }

        public float GetPercentile(float[] sequence, float percentile)
        {
            Array.Sort(sequence);
            
            var length = sequence.Length;
            var n = (length - 1) * percentile + 1;
            
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1.0f)
            {
                return sequence[0];
            }

            if (n == length)
            {
                return sequence[length - 1];
            }
            
            int k = (int)n;
            float d = n - k;
            
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }
    }
}
