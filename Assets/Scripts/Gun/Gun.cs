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
        }

        private void Update()
        {
            if (!GameManager.Instance.InputEnabled)
                return;
            
            if (objectToRotate == null || viewCamera == null)
            {
                return;
            }

            Vector2 screenPos;
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                screenPos = Mouse.current.position.ReadValue();
            }
            else
            {
                return;
            }
#else
        screenPos = Input.mousePosition;
#endif

            Vector3 worldPoint = viewCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cursorDepthFromCamera));
            objectToRotate.LookAt(worldPoint);
        }
    }
}