using UnityEngine;
using UnityEngine.InputSystem;

namespace Platform
{
    public class GamePointer : MonoBehaviour
    {
        public static Vector2 Pointer;
        public static bool ExternalOverrideActive; // Set true while a controller (e.g., PointerController) is managing pointer
    
        private static Vector2 _center;
        
        [SerializeField] private RectTransform _pointerTransform;

        private void Awake()
        {
            _center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            // On mobile, initialize pointer at screen center at start
#if UNITY_IOS || UNITY_ANDROID
        Pointer = _center;
#else
            Pointer = MousePosition();
#endif
        }

        private Vector2 MousePosition()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
        }

        private void Update()
        {
            // if (TouchController.UsingGyroOffset) 
            //     Pointer = _center + TouchController.PointerPosition;

            _pointerTransform.position = Pointer;
#if UNITY_IOS || UNITY_ANDROID
             
#else
            // Only pull from mouse if nothing else is actively overriding the pointer
            if (!ExternalOverrideActive && !TouchController.UsingGyroOffset)
                Pointer = MousePosition();
#endif
        }
    }
}
