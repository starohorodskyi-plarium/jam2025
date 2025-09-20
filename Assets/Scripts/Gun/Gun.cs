using Core;
using Platform;
using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Gun
{
    public class Gun : MonoBehaviour
    {
        [FormerlySerializedAs("objectToRotate")] [SerializeField] private Transform _objectToRotate;
        [FormerlySerializedAs("viewCamera")] [SerializeField] private Camera _viewCamera;
        [FormerlySerializedAs("cursorDepthFromCamera")] [SerializeField, Min(0.01f)] private float _cursorDepthFromCamera = 10f;
        [FormerlySerializedAs("aimWeight")] [SerializeField, Range(0f, 1f)] private float _aimWeight = 1f;
        private Quaternion _initialRotation;

        private void Awake()
        {
            if (_objectToRotate == null) 
                _objectToRotate = transform;

            if (_viewCamera == null) 
                _viewCamera = Camera.main;
            
            _initialRotation = _objectToRotate.rotation;
        }

        private void Update()
        {
            if (!GameManager.Instance.InputEnabled)
                return;
            
            if (_objectToRotate == null || _viewCamera == null)
                return;
            
            var worldPoint = _viewCamera.ScreenToWorldPoint(new Vector3(GamePointer.Pointer.x, GamePointer.Pointer.y, _cursorDepthFromCamera));
            var forward = worldPoint - _objectToRotate.position;
            if (!(forward.sqrMagnitude > 1e-6f))
                return;
            
            var targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            _objectToRotate.rotation = Quaternion.Slerp(_initialRotation, targetRotation, _aimWeight);
        }
    }
}