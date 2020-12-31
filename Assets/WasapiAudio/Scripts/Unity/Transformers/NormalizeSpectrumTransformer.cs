using System;

namespace Assets.WasapiAudio.Scripts.Unity.Transformers
{
    [Serializable]
    public class NormalizeSpectrumTransformer : SpectrumTransformer
    {
        protected override void PerformTransform(float[] input, float[] output)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var normalized = (input[i] - InputMin) / InputRange;
                output[i] = normalized;
            }
        }
    }
}
