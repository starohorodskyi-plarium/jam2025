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
        [Tooltip("If checked, this GameObject will be enabled on Web (WebGL). Mobile browsers count as Mobile, not Web.")]
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

        [Header("Extra conditions")]
        [Tooltip("If checked, this GameObject is also disabled when vibration is unavailable (iOS browser, desktop).")]
        public bool RequireVibrationSupport;

        private void Awake() =>
            ApplyActivationForCurrentPlatform();

        private void ApplyActivationForCurrentPlatform()
        {
            if (!Target)
            {
                Debug.LogWarning($"[{nameof(ObjectActivatorByPlatform)}] {name}: {nameof(Target)} is not assigned", this);
                return;
            }

            if (RequireVibrationSupport && !WGVibration.IsSupported)
            {
                Target.SetActive(false);
                return;
            }

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

        // Мобильный браузер считается Mobile, а не Web: там нужны те же тач-контролы, что в нативной сборке.
        private static bool IsMobilePlatform() =>
            Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer
            || WGPlatform.IsMobileBrowser;

        private static bool IsWebPlatform() =>
            Application.platform == RuntimePlatform.WebGLPlayer && !WGPlatform.IsMobileBrowser;
    }
}
