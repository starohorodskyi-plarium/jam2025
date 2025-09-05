using UnityEngine;

namespace Platform
{
    public class ObjectActivatorByPlatform : MonoBehaviour
    {
        public GameObject target;
        
        [Header("Enable on platforms")]
        [Tooltip("If checked, this GameObject will be enabled on PC (Standalone + Editor).")]
        public bool enableOnPC;

        [Tooltip("If checked, this GameObject will be enabled on Mobile (iOS/Android).")]
        public bool enableOnMobile;

        [Tooltip("If checked, this GameObject will be enabled on Web (WebGL).")]
        public bool enableOnWeb;

        [Header("Disable on platforms")]
        [Tooltip("If checked, this GameObject will be disabled on PC (Standalone + Editor). Overrides enable.")]
        public bool disableOnPC;

        [Tooltip("If checked, this GameObject will be disabled on Mobile (iOS/Android). Overrides enable.")]
        public bool disableOnMobile;

        [Tooltip("If checked, this GameObject will be disabled on Web (WebGL). Overrides enable.")]
        public bool disableOnWeb;

        private void Awake()
        {
            ApplyActivationForCurrentPlatform();
        }

        public void ApplyActivationForCurrentPlatform()
        {
            var isPC = IsPCPlatform();
            bool isMobile = IsMobilePlatform();
            bool isWeb = IsWebPlatform();

            bool shouldDisable = (isPC && disableOnPC) || (isMobile && disableOnMobile) || (isWeb && disableOnWeb);
            bool shouldEnable = (isPC && enableOnPC) || (isMobile && enableOnMobile) || (isWeb && enableOnWeb);

            if (shouldDisable)
            {
                target.SetActive(false);
                return;
            }

            if (shouldEnable)
            {
                target.SetActive(true);
                return;
            }

            // If neither enable nor disable is specified for this platform, keep the current state
        }

        private static bool IsPCPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsMobilePlatform() => 
            Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        private static bool IsWebPlatform() => 
            Application.platform == RuntimePlatform.WebGLPlayer;
    }
}
