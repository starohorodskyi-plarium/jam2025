using UnityEngine;

namespace Platform
{
    public class AndroidSettings : MonoBehaviour
    {
        private const int DefaultFrameRate = 60;
        private void Start()
        {
#if UNITY_ANDROID
            var frameRate = Screen.resolutions[0].refreshRateRatio.value;
            Application.targetFrameRate = double.IsNaN(frameRate) ? DefaultFrameRate:  (int)frameRate;
#endif
        }
    }
}
