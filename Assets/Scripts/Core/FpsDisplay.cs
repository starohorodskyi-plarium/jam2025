using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class FpsDisplay : MonoBehaviour
    {
        [FormerlySerializedAs("font")] public Font Font;
        [FormerlySerializedAs("showVersion")] public bool ShowVersion = true;
        private float _deltaTime;

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            var style = new GUIStyle();

            var rect = new Rect(80, 5, w, h * 50);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 4 / 100;
            style.normal.textColor = new Color(1f, 1f, 0.49f);
            style.font = Font;
            var fps = 1.0f / _deltaTime;
            var textVersion = $"  v.{Application.version}";
            var text = $"{fps:0.} FPS ";
            GUI.Label(rect, ShowVersion ? text + textVersion : text, style);
        }
    }
}