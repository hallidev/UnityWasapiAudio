using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [AddComponentMenu("VFX/Property Binders/VFX Audio Spectrum Data Binder")]
    [VFXBinder("Audio/Audio Spectrum Data to AttributeMap")]
    public class VfxAudioSpectrumDataBinder : VfxAudioVisualizationEffect
    {
        [VFXPropertyBinding("System.UInt32"), SerializeField]
        protected ExposedProperty SpectrumSizeParameter = "SpectrumSize";

        [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
        protected ExposedProperty SpectrumDataTextureParameter = "SpectrumDataTexture";

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