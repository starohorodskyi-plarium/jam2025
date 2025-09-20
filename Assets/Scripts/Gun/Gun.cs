using Core;
using Platform;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Gun
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Transform objectToRotate;
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.01f)] private float cursorDepthFromCamera = 10f;
        [SerializeField, Range(0f, 1f)] private float aimWeight = 1f;
        private Quaternion _initialRotation;

        private void Awake()
        {
            if (objectToRotate == null)
            {
                objectToRotate = transform;
            }

            if (viewCamera == null)
            {
                viewCamera = Camera.main;
            }
            _initialRotation = objectToRotate.rotation;
        }

        private void Update()
        {
            if (!GameManager.Instance.InputEnabled)
                return;
            
            if (objectToRotate == null || viewCamera == null)
            {
                return;
            }
            
            Vector3 worldPoint = viewCamera.ScreenToWorldPoint(new Vector3(GamePointer.Pointer.x, GamePointer.Pointer.y, cursorDepthFromCamera));
            Vector3 forward = worldPoint - objectToRotate.position;
            if (forward.sqrMagnitude > 1e-6f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
                objectToRotate.rotation = Quaternion.Slerp(_initialRotation, targetRotation, aimWeight);
            }
        }
    }
}