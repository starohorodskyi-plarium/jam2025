using UnityEngine;

namespace Platform
{
    public class PointerController : MonoBehaviour
    {
        [Header("Touchpad Area")] 
        [SerializeField] private RectTransform _pointerMoveControlArea;

        [Header("Movement Settings")] 
        [Tooltip("Multiplier for how much the pointer moves relative to finger delta (pixels).")] 
        [SerializeField] private float _sensitivity = 1.2f;
        [Tooltip("Smoothing time for SmoothDamp (ignored if DOTween is used).")] 
        [SerializeField] private float _smoothTime = 0.05f;
        [Tooltip("Clamp pointer to screen bounds.")] 
        [SerializeField] private bool _clampToScreen = true;
        [Tooltip("Also allow this to work in editor play mode for debugging.")] 
        [SerializeField] private bool _enableInEditor = true;
        
        [Header("Keyboard Control (Editor Only)")]
        [Tooltip("Speed of keyboard arrow key movement (pixels per second)")]
        [SerializeField] private float _keyboardSpeed = 500f;
        
        [Header("Gyro")] 
        [SerializeField] private GyroController _gyro;

        private int _activeTouchId = -1;
        
        private Vector2 _lastTouchPos;
        private Vector2 _targetPointer;
        private Vector2 _smoothVelocity;

        private bool PlatformActive
        {
            get
            {
#if UNITY_IOS || UNITY_ANDROID
                return true;
#elif UNITY_EDITOR
                return _enableInEditor; // Simulate on editor if desired
#else
                return false;
#endif
            }
        }

        private void Start() => 
            ResetMobilePointer();

        private void OnEnable()
        {
            _targetPointer = GamePointer.Pointer;
            if (PlatformActive)
                GamePointer.ExternalOverrideActive = true; // prevent mouse overwrite in editor
        }

        private void OnDisable()
        {
            _activeTouchId = -1;
            if (GamePointer.ExternalOverrideActive)
                GamePointer.ExternalOverrideActive = false;
        }

        private void Update()
        {
            if (!PlatformActive)
            {
                // Ensure flag cleared if platform deactivated at runtime (e.g. toggling _enableInEditor)
                if (GamePointer.ExternalOverrideActive)
                    GamePointer.ExternalOverrideActive = false;
                return;
            }

            if (!GamePointer.ExternalOverrideActive)
            {
                // Reassert if user enabled simulation in inspector during play
                GamePointer.ExternalOverrideActive = true;
            }

            ProcessTouchpad();
            
#if UNITY_EDITOR
            ProcessKeyboardInput();
#endif
            
            var finalTarget = _targetPointer;
            
            if (GyroController.UsingGyroOffset)
                finalTarget += GyroController.GyroInputOffset;
            
            if (_clampToScreen)
                finalTarget = ClampToScreen(finalTarget);
            
            if ((GamePointer.Pointer - finalTarget).sqrMagnitude > 0.01f) 
                GamePointer.Pointer = Vector2.Lerp(GamePointer.Pointer, finalTarget, _smoothTime);
        }

#if UNITY_EDITOR
        private void ProcessKeyboardInput()
        {
            var input = Vector2.zero;
            
            if (Input.GetKey(KeyCode.UpArrow))
                input.y += 1f;
            if (Input.GetKey(KeyCode.DownArrow))
                input.y -= 1f;
            if (Input.GetKey(KeyCode.LeftArrow))
                input.x -= 1f;
            if (Input.GetKey(KeyCode.RightArrow))
                input.x += 1f;
            
            if (input.sqrMagnitude > 0f)
            {
                var delta = input.normalized * (_keyboardSpeed * Time.deltaTime);
                ApplyPointerDelta(delta);
            }
        }
#endif

        private void ProcessTouchpad()
        {
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
            // Acquire or update active touch
            if (_activeTouchId == -1)
            {
                for (var i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    if (t.phase != TouchPhase.Began || !IsInsideArea(t.position)) 
                        continue;
                    
                    _activeTouchId = t.fingerId;
                    _lastTouchPos = t.position;
                    break;
                }
            }

            if (_activeTouchId != -1)
            {
                Touch? maybeTouch = null;
                for (var i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    if (t.fingerId != _activeTouchId)
                        continue;
                    
                    maybeTouch = t;
                    break;
                }

                if (!maybeTouch.HasValue)
                {
                    // Touch disappeared unexpectedly
                    _activeTouchId = -1;
                    return;
                }

                var touch = maybeTouch.Value;
                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                    {
                        var delta = touch.position - _lastTouchPos;
                        
                        if (touch.phase == TouchPhase.Moved)
                        {
                            if (delta.sqrMagnitude > 0f)
                            {
                                ApplyPointerDelta(delta * _sensitivity);
                                
                                _lastTouchPos = touch.position;
                            }
                        }
                        break;
                    }
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        _activeTouchId = -1;
                        break;
                }
            }
#endif
        }

        private void ApplyPointerDelta(Vector2 delta)
        {
            _targetPointer += delta;
            _targetPointer = ClampToScreen(_targetPointer);
        }

        private bool IsInsideArea(Vector2 screenPos) =>
            _pointerMoveControlArea != null 
            && RectTransformUtility.RectangleContainsScreenPoint(_pointerMoveControlArea, screenPos, null);

        private static Vector2 ClampToScreen(Vector2 p)
        {
            var x = Mathf.Clamp(p.x, 0f, Screen.width);
            var y = Mathf.Clamp(p.y, 0f, Screen.height);
            return new Vector2(x, y);
        }

        public void ResetMobilePointer()
        {
            _activeTouchId = -1;
            _targetPointer = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            GyroController.ResetGyro();
            
#if UNITY_IOS || UNITY_ANDROID
            GamePointer.ResetPointer();  
#endif
        }
    }
}
