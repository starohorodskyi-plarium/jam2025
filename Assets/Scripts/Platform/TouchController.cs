using UnityEngine;

public class TouchController : MonoBehaviour
{
    [Header("Gyro Aim (Mobile)")]
    [SerializeField] private float maxOffsetPixels = 150f;
    [SerializeField] private float gyroSensitivity = 2f;
    [SerializeField] private float smoothTime = 0.05f;

    private Vector2 _smoothedPointer;
    private Vector2 _smoothVelocity;

    public static Vector2 PointerPosition { get; private set; }
    public static bool UsingGyroOffset { get; private set; }

    void Start()
    {
#if UNITY_IOS || UNITY_ANDROID
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
#endif
    }

    void Update()
    {
#if UNITY_IOS || UNITY_ANDROID
        UpdateMobilePointerWithGyro();
#else
        PointerPosition = GamePointer.Pointer;
        UsingGyroOffset = false;
#endif
    }

#if UNITY_IOS || UNITY_ANDROID
    private void UpdateMobilePointerWithGyro()
    {
        // Base is the current active touch position (primary touch). If no touch, keep last.
        bool hasTouch = Input.touchCount > 0;
        Vector2 basePoint = hasTouch ? (Vector2)Input.GetTouch(0).position : _smoothedPointer;

        Vector2 offsetPixels = Vector2.zero;
        if (Input.gyro.enabled)
        {
            // Use gravity (device tilt) as a stable orientation source
            Vector3 g = Input.gyro.gravity; // ~[-1..1]

            // Map tilt to screen X/Y. Invert Y so device forward tilt moves cursor up
            Vector2 tilt = new Vector2(g.x, -g.y) * gyroSensitivity;

            // Clamp tilt magnitude and scale to pixels
            if (tilt.sqrMagnitude > 1f)
                tilt = tilt.normalized;

            offsetPixels = tilt * maxOffsetPixels;
        }

        Vector2 target = ClampToScreen(basePoint + offsetPixels);

        // Smooth for stability
        _smoothedPointer = Vector2.SmoothDamp(_smoothedPointer, target, ref _smoothVelocity, smoothTime);

        PointerPosition = _smoothedPointer;
        UsingGyroOffset = hasTouch && Input.gyro.enabled;
    }

    private static Vector2 ClampToScreen(Vector2 p)
    {
        float x = Mathf.Clamp(p.x, 0f, Screen.width);
        float y = Mathf.Clamp(p.y, 0f, Screen.height);
        return new Vector2(x, y);
    }
#endif
}
