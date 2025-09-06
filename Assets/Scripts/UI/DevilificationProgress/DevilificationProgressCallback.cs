using UnityEngine;

public class DevilificationProgressCallback : MonoBehaviour
{
    [SerializeField] private float progressValue;

    public void SetInstance()
    {
        DevilificationProgress.OnSetInstant?.Invoke(progressValue);
    }

    public void SetSmooth()
    {
        DevilificationProgress.OnSetSmooth?.Invoke(progressValue);
    }
}