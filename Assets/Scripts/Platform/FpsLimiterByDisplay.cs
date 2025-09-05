using UnityEngine;

namespace Platform
{
    public class FpsLimiterByDisplay : MonoBehaviour
    {
        [Header("Behavior")]
        [Tooltip("Apply only on Windows and macOS (including Editor).")]
        public bool applyOnlyOnDesktop = true;

        [Tooltip("Enable VSync to cap FPS to display refresh. On desktop this is the most reliable method.")]
        public bool enableVSync = true;

        [Tooltip("Also set Application.targetFrameRate to the detected refresh rate as a backup.")]
        public bool alsoSetTargetFrameRate = true;

        [Header("Dynamic updates")]
        [Tooltip("Poll the current display refresh rate in runtime and re-apply if it changes.")]
        public bool pollForChanges = true;

        [Tooltip("Seconds between refresh rate checks when polling is enabled.")]
        [Min(0.1f)]
        public float pollIntervalSeconds = 2f;

        [Header("Logging")]
        public bool verboseLogging;

        private int _lastAppliedFps = -1;

        private void OnEnable()
        {
            Apply();
            if (!pollForChanges) return;
            CancelInvoke(nameof(CheckAndReapplyIfChanged));
            InvokeRepeating(nameof(CheckAndReapplyIfChanged), pollIntervalSeconds, pollIntervalSeconds);
        }

        private void OnDisable()
        {
            if (!pollForChanges) return;
            CancelInvoke(nameof(CheckAndReapplyIfChanged));
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) Apply();
        }

        private void CheckAndReapplyIfChanged()
        {
            var hz = DetectCurrentDisplayRefreshRate();
            if (hz <= 0 || hz == _lastAppliedFps) return;
            ApplyInternal(hz);
        }

        public void Apply()
        {
            if (applyOnlyOnDesktop && !IsWindowsOrMacEditorOrPlayer())
            {
                if (verboseLogging) Debug.Log("[FpsLimiterByDisplay] Skipped (not Windows/macOS).");
                return;
            }

            var hz = DetectCurrentDisplayRefreshRate();
            if (hz <= 0)
            {
                if (verboseLogging) Debug.Log("[FpsLimiterByDisplay] Could not detect refresh rate. Skipping.");
                return;
            }

            ApplyInternal(hz);
        }

        private void ApplyInternal(int targetFps)
        {
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;
            
            if (alsoSetTargetFrameRate) Application.targetFrameRate = targetFps;
            _lastAppliedFps = targetFps;
            
            if (verboseLogging)
                Debug.Log($"[FpsLimiterByDisplay] Applied: vSyncCount={(enableVSync ? 1 : 0)}, targetFrameRate={Application.targetFrameRate}, detectedHz={targetFps}");
        }

        private static bool IsWindowsOrMacEditorOrPlayer() =>
            Application.platform is RuntimePlatform.WindowsPlayer
            or RuntimePlatform.OSXPlayer
            or RuntimePlatform.WindowsEditor
            or RuntimePlatform.OSXEditor;

        private static int DetectCurrentDisplayRefreshRate()
        {
            try
            {
                var rr = Screen.mainWindowDisplayInfo.refreshRate;
                if (rr.numerator > 0 && rr.denominator > 0)
                    return Mathf.RoundToInt((float)rr.value);
            }
            catch
            {
                // ignored
            }

            return 60;
        }
    }
}


