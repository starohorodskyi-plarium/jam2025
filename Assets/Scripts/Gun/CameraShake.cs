using System;
using DG.Tweening;
using UnityEngine;

namespace Gun
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform targetToShake;

        [Header("Shake Settings")]
        [SerializeField, Min(0.01f)] private float durationSeconds = 0.12f;
        [SerializeField] private Vector3 strength = new Vector3(0.15f, 0.15f, 0.15f);
        [SerializeField, Min(1)] private int vibrato = 20;
        [SerializeField, Range(0f, 180f)] private float randomness = 90f;
        [SerializeField] private bool snapping = false;
        [SerializeField] private bool fadeOut = true;

        [Header("Ease")]
        [SerializeField] private Ease ease = Ease.OutQuad;

        private Tween activeShakeTween;
        
        public static Action TriggerShake;

        private void Awake()
        {
            if (targetToShake == null)
            {
                targetToShake = transform;
            }
        }

        private void OnEnable()
        {
            TriggerShake += PlayShake;
        }

        private void OnDisable()
        {
            TriggerShake -= PlayShake;
        }

        public void PlayShake()
        {
            if (targetToShake == null)
            {
                return;
            }

            if (activeShakeTween != null && activeShakeTween.IsActive())
            {
                activeShakeTween.Kill(true);
            }

            activeShakeTween = targetToShake
                .DOShakePosition(durationSeconds, strength, vibrato, randomness, snapping, fadeOut)
                .SetEase(ease);
        }
    }
}


