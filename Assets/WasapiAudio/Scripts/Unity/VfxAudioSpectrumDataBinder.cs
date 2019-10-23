using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.Experimental.VFX.Utility;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [AddComponentMenu("VFX/Property Binders/VFX Audio Spectrum Data Binder")]
    [VFXBinder("Audio/Audio Spectrum Data to AttributeMap")]
    public class VfxAudioSpectrumDataBinder : VfxAudioVisualizationEffect
    {
        [VFXParameterBinding("System.UInt32"), SerializeField]
        protected ExposedParameter SpectrumSizeParameter = "SpectrumSize";

        [VFXParameterBinding("UnityEngine.Texture2D"), SerializeField]
        protected ExposedParameter SpectrumDataTextureParameter = "SpectrumDataTexture";

        private Texture2D _texture;
        private Color[] _colorCache;

        private void UpdateTexture()
        {
            if (_texture == null)
            {
                _texture = new Texture2D(SpectrumSize, 1, TextureFormat.RFloat, false);
                _texture.name = $"AudioSpectrumData{SpectrumSize}";
                _colorCache = new Color[SpectrumSize];
            }

            var spectrumData = GetSpectrumData();

            for (int i = 0; i < SpectrumSize; i++)
            {
                _colorCache[i] = new Color(spectrumData[i], 0, 0, 0);
            }

            _texture.SetPixels(_colorCache);
            _texture.Apply();
        }

        public override void UpdateBinding(VisualEffect component)
        {
            base.UpdateBinding(component);
            UpdateTexture();
            component.SetUInt(SpectrumSizeParameter, (uint) SpectrumSize);
            component.SetTexture(SpectrumDataTextureParameter, _texture);
        }

        public override string ToString()
        {
            return "Audio Spectrum Data";
        }
    }
}