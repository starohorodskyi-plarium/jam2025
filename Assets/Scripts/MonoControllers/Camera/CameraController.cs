using UnityEngine;
using DG.Tweening; // added for smooth reset

namespace MonoControllers.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        public float maxXAngle = 5f;       // max up/down tilt
        public float maxYAngle = 5f;       // max left/right tilt
        public float sensitivityX = 2f;    // horizontal sensitivity
        public float sensitivityY = 2f;    // vertical sensitivity
        public float smoothTime = 0.1f;    // smoothing time
        
        [Header("Limits")]
        public float minPitch = -10f; // looking down
        public float maxPitch = 10f;  // looking up
        public float minYaw = -10f;   // left
        public float maxYaw = 10f;  

        private Vector2 currentRotation;
        private Vector2 rotationVelocity;

        [Header("Reset Tween")] [SerializeField] private float _resetDuration = 0.35f; 
        [SerializeField] private Ease _resetEase = Ease.OutQuad;
        
        private Tween _resetTween; 
        private bool _isResetting;

        void Update()
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
                return;
            if (_isResetting) // while tweening back to zero, ignore input logic
            {
                return;
            }
            
            // Normalized mouse position (-1..1)
            float mouseX = (GamePointer.Pointer.x / Screen.width - 0.5f) * 2f;
            float mouseY = (GamePointer.Pointer.y / Screen.height - 0.5f) * 2f;

            // Target rotation scaled by sensitivity and clamped by max angles
            float targetX = -mouseY * maxXAngle * sensitivityY;
            float targetY =  mouseX * maxYAngle * sensitivityX;

            Vector2 targetRotation = new Vector2(targetX, targetY);

            // Smooth towards target
            currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);

            // Clamp rotation to limits
            currentRotation.x = Mathf.Clamp(currentRotation.x, minPitch, maxPitch);
            currentRotation.y = Mathf.Clamp(currentRotation.y, minYaw, maxYaw);

            // Apply rotation
            transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        }

        private void OnDisable()
        {
            if (_resetTween != null && _resetTween.IsActive())
            {
                _resetTween.Kill();
            }
            _isResetting = false;
        }

        public void ResetCameraRotationSmooth(float? customDuration = null)
        {
            _resetTween?.Kill();
            _isResetting = true;
            rotationVelocity = Vector2.zero; // stop smoothing momentum
            float duration = customDuration ?? _resetDuration;
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