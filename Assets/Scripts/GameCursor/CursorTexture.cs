using UnityEngine;

namespace GameCursor
{
    public class CursorTexture : MonoBehaviour
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 hotspot = Vector2.zero;
        [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

        public void SetDefaultCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void SetCustomCursor()
        {
            if (!cursorTexture)
            {
                Debug.LogWarning("CursorTexture: No cursor texture assigned in the Inspector.");
                return;
            }

            Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        }
    }
}


