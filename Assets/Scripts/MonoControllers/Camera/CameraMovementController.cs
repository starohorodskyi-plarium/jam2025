using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private bool smooth = true;
    [SerializeField] private float smoothSpeed = 10f;

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
        
        compensatedMinX = minX * compensation;
        compensatedMaxX = maxX * compensation;
    }

    private void Update()
    {
        var normalizedMouseX = 0.5f;
        
        if (Screen.width > 0)
            normalizedMouseX = Mathf.Clamp01(GamePointer.Pointer.x / Screen.width);

        var targetX = Mathf.Lerp(compensatedMinX, compensatedMaxX, normalizedMouseX);

        var current = transform.position;
        float newX;
        
        if (smooth)
        {
            var t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            newX = Mathf.Lerp(current.x, targetX, t);
        }
        else
        {
            newX = targetX;
        }

        transform.position = new Vector3(newX, current.y, current.z);
    }

    void OnValidate()
    {
        if (compensatedMaxX < compensatedMinX)
            (compensatedMinX, compensatedMaxX) = (compensatedMaxX, compensatedMinX);

        if (smoothSpeed < 0f)
            smoothSpeed = 0f;
    }
}
