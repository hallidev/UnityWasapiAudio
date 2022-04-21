using System.Collections.Generic;
using System.Linq;
using Assets.WasapiAudio.Scripts.Core;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [ExecuteInEditMode]
    public class WasapiAudioSource : MonoBehaviour
    {
        private Core.WasapiAudio _wasapiAudio;
        private float[] _spectrumData;

        public bool IsIdle => _spectrumData.All(v => v < 0.001f);

        // Inspector Properties
        public WasapiCaptureType CaptureType = WasapiCaptureType.Loopback;
        public int SpectrumSize = 32;
        public ScalingStrategy ScalingStrategy = ScalingStrategy.Sqrt;
        public WindowFunctionType WindowFunctionType = WindowFunctionType.BlackmannHarris;
        public int MinFrequency = 100;
        public int MaxFrequency = 20000;
        public WasapiAudioFilter[] Filters;

        public void Awake()
        {
            var spectra = new List<SpectrumDescriptor>();

            spectra.Add(new SpectrumDescriptor("Key", SpectrumSize, ScalingStrategy, WindowFunctionType, MinFrequency, MaxFrequency, spectrumData =>
            {
                _spectrumData = spectrumData;
            }));

            // Setup loopback audio and start listening
            _wasapiAudio = new Core.WasapiAudio(CaptureType, Filters, spectra);

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
