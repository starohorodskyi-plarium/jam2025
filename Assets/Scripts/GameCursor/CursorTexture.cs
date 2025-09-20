using UnityEngine;
using UnityEngine.Serialization;

namespace GameCursor
{
    public class CursorTexture : MonoBehaviour
    {
        [FormerlySerializedAs("cursorTexture")] [SerializeField] private Texture2D _cursorTexture;
        [FormerlySerializedAs("cursorZoomTexture")] [SerializeField] private Texture2D _cursorZoomTexture;
        [FormerlySerializedAs("hotspot")] [SerializeField] private Vector2 _hotspot = Vector2.zero;
        [FormerlySerializedAs("cursorMode")] [SerializeField] private CursorMode _cursorMode = CursorMode.Auto;

        public void SetDefaultCursor() => 
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        public void SetCustomCursor()
        {
            if (!_cursorTexture)
            {
                Debug.LogWarning("CursorTexture: No cursor texture assigned in the Inspector.");
                return;
            }

            Cursor.SetCursor(_cursorTexture, _hotspot, _cursorMode);
        }
        
        public void SetCustomZoomCursor()
        {
            if (!_cursorZoomTexture)
            {
                Debug.LogWarning("CursorTexture: No cursor texture assigned in the Inspector.");
                return;
            }

            Cursor.SetCursor(_cursorZoomTexture, _hotspot, _cursorMode);
        }
    }
}


