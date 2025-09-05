using DG.Tweening;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetToRecoil;

    [Header("Recoil Settings")] 
    [SerializeField, Range(0.1f, 45f)] private float recoilAngleDegrees = 6f;
    [SerializeField, Min(0.01f)] private float durationSeconds = 0.12f;
    [SerializeField, Min(1)] private int vibrato = 12;
    [SerializeField, Range(0f, 1f)] private float elasticity = 0.5f;

    [Header("Ease")] 
    [SerializeField] private Ease ease = Ease.OutQuad;

    private Tween activeRecoilTween;

    private void Awake()
    {
        if (targetToRecoil == null)
        {
            targetToRecoil = transform;
        }
    }

    public void PlayRecoil()
    {
        if (targetToRecoil == null)
        {
            return;
        }

        if (activeRecoilTween != null && activeRecoilTween.IsActive())
        {
            activeRecoilTween.Kill(true);
        }

        Vector3 punch = new Vector3(-Mathf.Abs(recoilAngleDegrees), 0f, 0f);
        activeRecoilTween = targetToRecoil
            .DOPunchRotation(punch, durationSeconds, vibrato, elasticity)
            .SetEase(ease);
    }
}
