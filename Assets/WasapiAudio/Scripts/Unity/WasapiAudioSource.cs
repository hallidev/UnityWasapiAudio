using Assets.WasapiAudio.Scripts.Core;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [ExecuteInEditMode]
    public class WasapiAudioSource : MonoBehaviour
    {
        private Core.WasapiAudio _wasapiAudio;
 
        // Inspector Properties
        public WasapiCaptureType CaptureType = WasapiCaptureType.Loopback;
        public WasapiAudioFilter[] Filters;

        public void Awake()
        {
            // Setup loopback audio and start listening
            _wasapiAudio = new Core.WasapiAudio(CaptureType, Filters);

            _wasapiAudio.StartCapture();
        }

        public void OnApplicationQuit()
        {
            _wasapiAudio?.StopCapture();
        }

        public void AddReceiver(SpectrumReceiver receiver)
        {
            _wasapiAudio.AddReceiver(receiver);
        }
    }
}
