using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ShootCallback : MonoBehaviour
{
    [SerializeField] private GameObject shootSoundPrefab;
    [SerializeField] private UnityEvent shootEvent;

    private void Update()
    {
        if (!shootSoundPrefab)
        {
            return;
        }

#if ENABLE_INPUT_SYSTEM
        bool pressed = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                       || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
#else
        bool pressed = Input.GetMouseButtonDown(0);
        if (!pressed && Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    pressed = true;
                    break;
                }
            }
        }
#endif

        if (pressed)
        {
            shootEvent?.Invoke();
            Instantiate(shootSoundPrefab, transform.position, Quaternion.identity);
        }
    }
}
