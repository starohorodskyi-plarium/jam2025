using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private bool smooth = true;
    [SerializeField] private float smoothSpeed = 10f;

    void Update()
    {
        float normalizedMouseX = 0.5f;
        if (Screen.width > 0)
        {
            normalizedMouseX = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
        }

        float targetX = Mathf.Lerp(minX, maxX, normalizedMouseX);

        Vector3 current = transform.position;
        float newX;
        if (smooth)
        {
            float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
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
        if (maxX < minX)
        {
            float tmp = minX;
            minX = maxX;
            maxX = tmp;
        }

        if (smoothSpeed < 0f)
        {
            smoothSpeed = 0f;
        }
    }
}
