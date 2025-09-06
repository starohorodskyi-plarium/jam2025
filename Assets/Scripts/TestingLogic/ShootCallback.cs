using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ShootCallback : MonoBehaviour
{
    public UnityEvent OnShootPressed;
    
    private bool IgnoreInputs => GameManager.Instance.CurrentState != GameManager.GameState.Playing;
    
    private void Update()
    {
        if (IgnoreInputs)
            return;

#if ENABLE_INPUT_SYSTEM
        var pressed = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
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
            OnShootPressed?.Invoke();
    }
}
