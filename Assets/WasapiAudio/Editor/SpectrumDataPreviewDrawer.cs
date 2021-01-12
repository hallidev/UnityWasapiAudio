using System.Linq;
using Assets.WasapiAudio.Scripts.Unity;
using UnityEditor;
using UnityEngine;

namespace Assets.WasapiAudio.Editor
{
    [CustomPropertyDrawer(typeof(SpectrumDataPreviewAttribute))]
    public class SpectrumDataPreviewDrawer : PropertyDrawer
    {
        private const float DrawerHeight = 150.0f;
        private const float AverageBarHeight = 5.0f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return DrawerHeight;
        }

        // Start is called before the first frame update
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject == null || property.serializedObject.targetObject == null)
            {
                return;
            }

            var preview = fieldInfo.GetValue(property.serializedObject.targetObject) as SpectrumData;

            if (preview == null || preview.Values == null)
            {
                return;
            }

            try
            {
                // Draw black background
                EditorGUI.DrawRect(position, Color.black);

                var x = 0.0f;
                var barWidth = position.width / preview.Values.Length;
                var average = preview.Values.Average() * DrawerHeight;

                for (var i = 0; i < preview.Values.Length; i++)
                {
                    // Draw spectrum bar
                    var barHeight = preview.Values[i] * DrawerHeight;

                    var spectrumBarColor = Color.green;

                    if (barHeight >= average)
                    {
                        spectrumBarColor = Color.yellow;
                    }

                    var spectrumBar = new Rect(position.x + x,
                        position.y + DrawerHeight - barHeight,
                        barWidth,
                        barHeight);

                    EditorGUI.DrawRect(spectrumBar, spectrumBarColor);

                    // Draw average line
                    var averageLine = new Rect(position.x,
                        position.y + DrawerHeight - average,
                        position.width,
                        AverageBarHeight);

                    EditorGUI.DrawRect(averageLine, Color.blue);

                    x += barWidth;
                }

                EditorGUI.PrefixLabel(position, label);
            }
            catch
            {

            }
        }
    }
}
