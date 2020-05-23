using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    public class AudioVizDebugGizmo : AudioVisualizationEffect
    {
        public float BarWidth = 0.1f;
        public float Scale = 1.0f;
        public float Power = 1.0f;

        public void Start()
        {
            
        }

        public void OnDrawGizmos()
        {
            var spectrumData = GetSpectrumData();

            for (var i = 0; i < SpectrumSize; i++)
            {
                var value = Mathf.Pow(spectrumData[i] * Scale, Power);
                var position = new Vector3(BarWidth * i, value / 2.0f, 0);
                Gizmos.color = Color.green;
                Gizmos.DrawCube(Vector3.Scale(position + transform.position, transform.localScale), Vector3.Scale(new Vector3(BarWidth, value, 0.2f), transform.localScale));
                Gizmos.color = Color.red;
            }
        }
    }
}
