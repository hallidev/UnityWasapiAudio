using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public class AudioVizDebugGizmo : AudioVisualizationEffect
    {
        public float Scale = 1.0f;
        public float Power = 1.0f;

        public void Start()
        {
            
        }

        public void OnDrawGizmos()
        {
            var maxHeight = 1.0f;
            var width = 0.1f;
            var spectrumData = GetSpectrumData();

            for (var i = 0; i < SpectrumSize; i++)
            {
                var value = Mathf.Pow(spectrumData[i] * maxHeight * Scale, Power);
                var position = new Vector3(width * i, value / 2.0f, 0);
                Gizmos.color = Color.green;
                Gizmos.DrawCube(Vector3.Scale(position + transform.position, transform.localScale), Vector3.Scale(new Vector3(width, value, 0.2f), transform.localScale));
                Gizmos.color = Color.red;
            }
        }
    }
}
