using System.Linq;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [ExecuteInEditMode]
    public class AudioVizDebugGizmo : AudioVisualizationEffect
    {
        public float BarWidth = 0.1f;
        public float Scale = 1.0f;

        public void Start()
        {
            
        }

        public void OnDrawGizmos()
        {
            var spectrumData = GetSpectrumData();
            var average = spectrumData.Average();
            
            // Draw a red measurement bar to indicate max
            Gizmos.color = Color.red;
            var measurementBarPosition = new Vector3(0, Scale / 2.0f, 0);
            Gizmos.DrawCube(Vector3.Scale(measurementBarPosition + transform.position, transform.localScale), Vector3.Scale(new Vector3(BarWidth, Scale, BarWidth), transform.localScale));

            for (var i = 0; i < SpectrumSize; i++)
            {
                var value = spectrumData[i] * Scale;
                var position = new Vector3(BarWidth * (i + 1), value / 2.0f, 0);
                Gizmos.color = Color.green;

                if (spectrumData[i] > average)
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawCube(Vector3.Scale(position + transform.position, transform.localScale), Vector3.Scale(new Vector3(BarWidth, value, BarWidth), transform.localScale));
            }

            // Draw a line indicating average
            Gizmos.color = Color.blue;
            var totalWidth = BarWidth * SpectrumSize + BarWidth;
            var averagePosition = new Vector3(totalWidth / 2.0f, average * Scale, 0);
            Gizmos.DrawCube(Vector3.Scale(averagePosition + transform.position, transform.localScale), Vector3.Scale(new Vector3(totalWidth, BarWidth / 2.0f, BarWidth), transform.localScale));

#if UNITY_EDITOR
            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }
    }
}
