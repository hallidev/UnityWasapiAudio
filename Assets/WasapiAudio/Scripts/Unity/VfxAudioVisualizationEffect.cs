using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public abstract class VfxAudioVisualizationEffect : VFXBinderBase
    {
        // Inspector Properties
        public WasapiAudioSource WasapiAudioSource;

        [SerializeReference]
        [SerializeReferenceButton]
        public List<SpectrumTransformer> Transformers = new List<SpectrumTransformer>();

        protected int SpectrumSize { get; private set; }

        public override bool IsValid(VisualEffect component)
        {
            return WasapiAudioSource != null;
        }

        public override void UpdateBinding(VisualEffect component)
        {
            SpectrumSize = WasapiAudioSource.SpectrumSize;
        }

        protected float[] GetSpectrumData()
        {
            // Get raw / unmodified spectrum data
            var spectrumData = WasapiAudioSource.GetSpectrumData();

            // Run spectrum data through all configured transformers
            if (Transformers != null && Transformers.Count > 0)
            {
                foreach (var transformer in Transformers)
                {
                    spectrumData = transformer.Transform(spectrumData);
                }
            }

            return spectrumData;
        }
    }
}
