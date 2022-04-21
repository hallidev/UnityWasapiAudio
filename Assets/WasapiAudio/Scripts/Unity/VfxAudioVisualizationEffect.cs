using System.Collections.Generic;
using Assets.WasapiAudio.Scripts.Core;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public abstract class VfxAudioVisualizationEffect : VFXBinderBase
    {
        private float[] _spectrumData;
        private SpectrumReceiver _receiver;

        // Inspector Properties
        public WasapiAudioSource WasapiAudioSource;
        public int SpectrumSize = 32;
        public ScalingStrategy ScalingStrategy = ScalingStrategy.Sqrt;
        public WindowFunctionType WindowFunctionType = WindowFunctionType.BlackmannHarris;
        public int MinFrequency = 100;
        public int MaxFrequency = 20000;

        [SerializeReference]
        [SerializeReferenceButton]
        public List<SpectrumTransformer> Transformers = new();

        [SpectrumDataPreview]
        public SpectrumData Preview;

        public override bool IsValid(VisualEffect component)
        {
            if (WasapiAudioSource == null)
            {
                return false;
            }

            if (_receiver == null)
            {
                _receiver = new SpectrumReceiver(SpectrumSize, ScalingStrategy, WindowFunctionType, MinFrequency,
                    MaxFrequency, spectrumData =>
                    {
                        _spectrumData = spectrumData;
                    });

                WasapiAudioSource.AddReceiver(_receiver);
            }

            return true;
        }

        public override void UpdateBinding(VisualEffect component)
        {
            Preview = new SpectrumData();
        }

        protected float[] GetSpectrumData()
        {
            if (_spectrumData == null)
            {
                return new float[SpectrumSize];
            }

            // Get raw / unmodified spectrum data
            var spectrumData = _spectrumData;

            // Run spectrum data through all configured transformers
            if (Transformers != null && Transformers.Count > 0)
            {
                foreach (var transformer in Transformers)
                {
                    spectrumData = transformer.Transform(spectrumData);
                }
            }

            Preview.Values = spectrumData;

            return spectrumData;
        }
    }
}
