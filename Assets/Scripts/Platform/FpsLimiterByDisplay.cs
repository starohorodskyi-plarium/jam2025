using UnityEngine;
using UnityEngine.Serialization;

namespace Platform
{
    public class FpsLimiterByDisplay : MonoBehaviour
    {
        [Header("Behavior")]
        [FormerlySerializedAs("applyOnlyOnDesktop")]
        [Tooltip("Apply only on Windows and macOS (including Editor).")]
        public bool ApplyOnlyOnDesktop = true;

        [FormerlySerializedAs("enableVSync")] 
        [Tooltip("Enable VSync to cap FPS to display refresh. On desktop this is the most reliable method.")]
        public bool EnableVSync = true;

        [FormerlySerializedAs("alsoSetTargetFrameRate")] 
        [Tooltip("Also set Application.targetFrameRate to the detected refresh rate as a backup.")]
        public bool AlsoSetTargetFrameRate = true;
        
        [Header("Dynamic updates")]
        [FormerlySerializedAs("pollForChanges")]
        [Tooltip("Poll the current display refresh rate in runtime and re-apply if it changes.")]
        public bool PollForChanges = true;

        [FormerlySerializedAs("pollIntervalSeconds")]
        [Tooltip("Seconds between refresh rate checks when polling is enabled.")]
        [Min(0.1f)]
        public float PollIntervalSeconds = 2f;

        [Header("Logging")]
        [FormerlySerializedAs("verboseLogging")]public bool VerboseLogging;

        private int _lastAppliedFps = -1;

        private void OnEnable()
        {
            Apply();
            
            if (!PollForChanges) 
                return;
            
            CancelInvoke(nameof(CheckAndReapplyIfChanged));
            InvokeRepeating(nameof(CheckAndReapplyIfChanged), PollIntervalSeconds, PollIntervalSeconds);
        }

        private void OnDisable()
        {
            if (!PollForChanges) 
                return;
            
            CancelInvoke(nameof(CheckAndReapplyIfChanged));
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) 
                Apply();
        }

        private void CheckAndReapplyIfChanged()
        {
            var hz = DetectCurrentDisplayRefreshRate();
            if (hz <= 0 || hz == _lastAppliedFps) return;
            ApplyInternal(hz);
        }

        private void Apply()
        {
            if (ApplyOnlyOnDesktop && !IsWindowsOrMacEditorOrPlayer())
            {
                if (VerboseLogging) Debug.Log("[FpsLimiterByDisplay] Skipped (not Windows/macOS).");
                return;
            }

            var hz = DetectCurrentDisplayRefreshRate();
            if (hz <= 0)
            {
                if (VerboseLogging) Debug.Log("[FpsLimiterByDisplay] Could not detect refresh rate. Skipping.");
                return;
            }

            ApplyInternal(hz);
        }

        private void ApplyInternal(int targetFps)
        {
            QualitySettings.vSyncCount = EnableVSync ? 1 : 0;
            
            if (AlsoSetTargetFrameRate) Application.targetFrameRate = targetFps;
            _lastAppliedFps = targetFps;
            
            if (VerboseLogging)
                Debug.Log($"[FpsLimiterByDisplay] Applied: vSyncCount={(EnableVSync ? 1 : 0)}, targetFrameRate={Application.targetFrameRate}, detectedHz={targetFps}");
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


