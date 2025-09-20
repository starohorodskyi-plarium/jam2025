using Platform;
using UnityEngine;
using UnityEngine.Serialization;

namespace MonoControllers.Camera
{
    public class CameraMovementController : MonoBehaviour
    {
        [FormerlySerializedAs("minX")] [SerializeField] private float _minX = -5f;
        [FormerlySerializedAs("maxX")] [SerializeField] private float _maxX = 5f;
        [FormerlySerializedAs("smooth")] [SerializeField] private bool _smooth = true;
        [FormerlySerializedAs("smoothSpeed")] [SerializeField] private float _smoothSpeed = 10f;

        [SerializeField] private AnimationCurve _screenAspectRatioCompensator;

        private float compensatedMinX;
        private float compensatedMaxX;

        private void Start() =>
            UpdateCompensation();

        public void SetCompensation(AnimationCurve screenAspectRatioCompensator)
        {
            _screenAspectRatioCompensator = screenAspectRatioCompensator;
            UpdateCompensation();
        }

        private void UpdateCompensation()
        {
            var aspectRatio = (float)Screen.width / Screen.height;
            var compensation = _screenAspectRatioCompensator.Evaluate(aspectRatio);

            compensatedMinX = _minX * compensation;
            compensatedMaxX = _maxX * compensation;
        }

        private void Update()
        {
            var normalizedMouseX = 0.5f;

            if (Screen.width > 0)
                normalizedMouseX = Mathf.Clamp01(GamePointer.Pointer.x / Screen.width);

            var targetX = Mathf.Lerp(compensatedMinX, compensatedMaxX, normalizedMouseX);

            var current = transform.position;
            float newX;

            if (_smooth)
            {
                var t = 1f - Mathf.Exp(-_smoothSpeed * Time.deltaTime);
                newX = Mathf.Lerp(current.x, targetX, t);
            }
            else
                newX = targetX;

            transform.position = new Vector3(newX, current.y, current.z);
        }

        private void OnValidate()
        {
            if (compensatedMaxX < compensatedMinX)
                (compensatedMinX, compensatedMaxX) = (compensatedMaxX, compensatedMinX);

            if (_smoothSpeed < 0f)
                _smoothSpeed = 0f;
        }
    }
}
