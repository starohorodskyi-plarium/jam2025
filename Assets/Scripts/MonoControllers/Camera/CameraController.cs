using Core;
using UnityEngine;
using DG.Tweening;
using Platform;
using UnityEngine.Serialization; // added for smooth reset

namespace MonoControllers.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [FormerlySerializedAs("maxXAngle")] public float MaxXAngle = 5f;       // max up/down tilt
        [FormerlySerializedAs("maxYAngle")] public float MaxYAngle = 5f;       // max left/right tilt
        [FormerlySerializedAs("sensitivityX")] public float SensitivityX = 2f;    // horizontal sensitivity
        [FormerlySerializedAs("sensitivityY")] public float SensitivityY = 2f;    // vertical sensitivity
        [FormerlySerializedAs("smoothTime")] public float SmoothTime = 0.1f;    // smoothing time
        
        [Header("Limits")]
        [FormerlySerializedAs("minPitch")] public float MinPitch = -10f; // looking down
        [FormerlySerializedAs("maxPitch")] public float MaxPitch = 10f;  // looking up
        [FormerlySerializedAs("minYaw")] public float MinYaw = -10f;   // left
        [FormerlySerializedAs("maxYaw")] public float MaxYaw = 10f;  

        private Vector2 currentRotation;
        private Vector2 rotationVelocity;

        [Header("Reset Tween")] [SerializeField] private float _resetDuration = 0.35f; 
        [SerializeField] private Ease _resetEase = Ease.OutQuad;
        
        private Tween _resetTween; 
        private bool _isResetting;

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
                return;
            
            if (_isResetting) // while tweening back to zero, ignore input logic
                return;
            
            // Normalized mouse position (-1..1)
            var mouseX = (GamePointer.Pointer.x / Screen.width - 0.5f) * 2f;
            var mouseY = (GamePointer.Pointer.y / Screen.height - 0.5f) * 2f;

            // Target rotation scaled by sensitivity and clamped by max angles
            var targetX = -mouseY * MaxXAngle * SensitivityY;
            var targetY =  mouseX * MaxYAngle * SensitivityX;

            var targetRotation = new Vector2(targetX, targetY);

            // Smooth towards target
            currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, SmoothTime);

            // Clamp rotation to limits
            currentRotation.x = Mathf.Clamp(currentRotation.x, MinPitch, MaxPitch);
            currentRotation.y = Mathf.Clamp(currentRotation.y, MinYaw, MaxYaw);

            // Apply rotation
            transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        }

        private void OnDisable()
        {
            if (_resetTween != null && _resetTween.IsActive()) 
                _resetTween.Kill();
            
            _isResetting = false;
        }

        public void ResetCameraRotationSmooth(float? customDuration = null)
        {
            _resetTween?.Kill();
            _isResetting = true;
            rotationVelocity = Vector2.zero; // stop smoothing momentum
            var duration = customDuration ?? _resetDuration;
            
            _resetTween = DOTween.To(() => currentRotation, v =>
            {
                currentRotation = v;
                transform.localRotation = Quaternion.Euler(v.x, v.y, 0f);
            }, Vector2.zero, duration)
            .SetEase(_resetEase)
            .OnComplete(() => { _isResetting = false; currentRotation = Vector2.zero; });
        }
    }
}