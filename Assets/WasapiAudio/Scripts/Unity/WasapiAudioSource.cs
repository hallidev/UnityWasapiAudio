using Assets.WasapiAudio.Scripts.Core;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [ExecuteInEditMode]
    public class WasapiAudioSource : MonoBehaviour
    {
        private Core.WasapiAudio _wasapiAudio;
        private float[] _spectrumData;

        // Inspector Properties
        public WasapiCaptureType CaptureType = WasapiCaptureType.Loopback;
        public int SpectrumSize = 32;
        public ScalingStrategy ScalingStrategy = ScalingStrategy.Sqrt;
        public int MinFrequency = 100;
        public int MaxFrequency = 20000;
        public WasapiAudioFilter[] Filters;

        public void Awake()
        {
            // Setup loopback audio and start listening
            _wasapiAudio = new Core.WasapiAudio(CaptureType, SpectrumSize, ScalingStrategy, MinFrequency, MaxFrequency, Filters, spectrumData =>
            {
                _spectrumData = spectrumData;
            });

            _wasapiAudio.StartListen();
        }

        public void Update()
        {

        }

        public float[] GetSpectrumData()
        {
            return _spectrumData;
        }

        public void OnApplicationQuit()
        {
            _wasapiAudio?.StopListen();
        }
    }
}
