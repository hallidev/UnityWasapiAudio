using System.Collections.Generic;
using System.Linq;
using Assets.WasapiAudio.Scripts.Core;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public abstract class AudioVisualizationEffect : MonoBehaviour
    {
        private float[] _spectrumData;

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

        protected bool IsIdle => _spectrumData?.All(v => v < 0.001f) ?? true;

        public virtual void Awake()
        {
            if (WasapiAudioSource == null)
            {
                Debug.Log("You must set a WasapiAudioSource");
                return;
            }

            var receiver = new SpectrumReceiver(SpectrumSize, ScalingStrategy, WindowFunctionType, MinFrequency,
                MaxFrequency, spectrumData =>
                {
                    _spectrumData = spectrumData;
                });

            WasapiAudioSource.AddReceiver(receiver);

            Preview = new SpectrumData();
        }

        protected float[] GetSpectrumData()
        {
            if (WasapiAudioSource == null)
            {
                Debug.Log("You must set a WasapiAudioSource");
                return null;
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
