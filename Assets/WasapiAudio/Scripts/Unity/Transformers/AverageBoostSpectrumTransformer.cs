using System;

namespace Assets.WasapiAudio.Scripts.Unity.Transformers
{
    [Serializable]
    public class AverageBoostSpectrumTransformer : SpectrumTransformer
    {
        public float BelowAverageMultiplier;
        public float AboveAverageMultiplier;

        protected override void PerformTransform(float[] input, ref float[] output)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] <= InputAverage)
                {
                    output[i] = input[i] * BelowAverageMultiplier;
                }
                else
                {
                    output[i] = input[i] * AboveAverageMultiplier;
                }
            }
        }
    }
}
