using UnityEngine;
using UnityEngine.InputSystem;

namespace Platform
{
    public class GamePointer : MonoBehaviour
    {
        public static Vector2 Pointer;
    
        private static Vector2 Center;

        private void Awake()
        {
            Center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            // On mobile, initialize pointer at screen center at start
#if UNITY_IOS || UNITY_ANDROID
        Pointer = Center;
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
            if (TouchController.UsingGyroOffset) {
                Pointer = Center + TouchController.PointerPosition;
            }
       
#if UNITY_IOS || UNITY_ANDROID

#else
            Pointer = MousePosition();
#endif
        }
    }
}
