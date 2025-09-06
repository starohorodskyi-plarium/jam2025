using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float maxXAngle = 5f;       // max up/down tilt
    public float maxYAngle = 5f;       // max left/right tilt
    public float sensitivityX = 2f;    // horizontal sensitivity
    public float sensitivityY = 2f;    // vertical sensitivity
    public float smoothTime = 0.1f;    // smoothing time

    private Vector2 currentRotation;
    private Vector2 rotationVelocity;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;
        
        // Normalized mouse position (-1..1)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Target rotation scaled by sensitivity and clamped by max angles
        float targetX = -mouseY * maxXAngle * sensitivityY;
        float targetY =  mouseX * maxYAngle * sensitivityX;

        Vector2 targetRotation = new Vector2(targetX, targetY);

        // Smooth towards target
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);

        // Apply rotation
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
    }
}