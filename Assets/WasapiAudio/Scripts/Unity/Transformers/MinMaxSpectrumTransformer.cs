using System;

namespace Assets.WasapiAudio.Scripts.Unity.Transformers
{
    [Serializable]
    public class MinMaxSpectrumTransformer : SpectrumTransformer
    {
        public float MinMultiplier;
        public float MaxMultiplier;

        protected override void PerformTransform(float[] input, ref float[] output)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] <= InputAverage)
                {
                    output[i] = input[i] * MinMultiplier;
                }
                else
                {
                    output[i] = input[i] * MaxMultiplier;
                }
            }
        }
    }
}
