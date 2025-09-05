#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Platform
{
    public class WebEngineLoadedCallback : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void EngineLoaded();
#endif

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            EngineLoaded();
#endif
        }
    }
}
