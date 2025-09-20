using UnityEngine;
using UnityEngine.Serialization;

namespace Platform
{
    public class ObjectActivatorByPlatform : MonoBehaviour
    {
        [FormerlySerializedAs("target")] public GameObject Target;
        
        [Header("Enable on platforms")]
        [FormerlySerializedAs("enableOnPC")]
        [Tooltip("If checked, this GameObject will be enabled on PC (Standalone + Editor).")]
        public bool EnableOnPC;

        [FormerlySerializedAs("enableOnMobile")] 
        [Tooltip("If checked, this GameObject will be enabled on Mobile (iOS/Android).")]
        public bool EnableOnMobile;

        [FormerlySerializedAs("enableOnWeb")] 
        [Tooltip("If checked, this GameObject will be enabled on Web (WebGL).")]
        public bool EnableOnWeb;
        
        [Header("Disable on platforms")]
        [FormerlySerializedAs("disableOnPC")]
        [Tooltip("If checked, this GameObject will be disabled on PC (Standalone + Editor). Overrides enable.")]
        public bool DisableOnPC;

        [FormerlySerializedAs("disableOnMobile")] 
        [Tooltip("If checked, this GameObject will be disabled on Mobile (iOS/Android). Overrides enable.")]
        public bool DisableOnMobile;

        [FormerlySerializedAs("disableOnWeb")] 
        [Tooltip("If checked, this GameObject will be disabled on Web (WebGL). Overrides enable.")]
        public bool DisableOnWeb;

        private void Awake() => 
            ApplyActivationForCurrentPlatform();

        private void ApplyActivationForCurrentPlatform()
        {
            var isPC = IsPCPlatform();
            var isMobile = IsMobilePlatform();
            var isWeb = IsWebPlatform();

            var shouldDisable = (isPC && DisableOnPC) || (isMobile && DisableOnMobile) || (isWeb && DisableOnWeb);
            var shouldEnable = (isPC && EnableOnPC) || (isMobile && EnableOnMobile) || (isWeb && EnableOnWeb);

            if (shouldDisable)
            {
                Target.SetActive(false);
                return;
            }

            if (!shouldEnable) 
                return;
            
            Target.SetActive(true);

            // If neither enable nor disable is specified for this platform, keep the current state
        }

        private static bool IsPCPlatform()
        {
            return Application.platform switch
            {
                RuntimePlatform.WindowsPlayer or RuntimePlatform.OSXPlayer or RuntimePlatform.LinuxPlayer
                    or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor
                    or RuntimePlatform.LinuxEditor => true,
                _ => false
            };
        }

        private static bool IsMobilePlatform() => 
            Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        private static bool IsWebPlatform() => 
            Application.platform == RuntimePlatform.WebGLPlayer;
    }
}
