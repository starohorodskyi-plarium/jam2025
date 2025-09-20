using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gun
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Target")]
        [FormerlySerializedAs("targetToShake")] [SerializeField] private Transform _targetToShake;
        
        [Header("Shake Settings")]
        [FormerlySerializedAs("durationSeconds")] [SerializeField, Min(0.01f)] private float _durationSeconds = 0.12f;
        [FormerlySerializedAs("strength")] [SerializeField] private Vector3 _strength = new(0.15f, 0.15f, 0.15f);
        [FormerlySerializedAs("vibrato")] [SerializeField, Min(1)] private int _vibrato = 20;
        [FormerlySerializedAs("randomness")] [SerializeField, Range(0f, 180f)] private float _randomness = 90f;
        [FormerlySerializedAs("snapping")] [SerializeField] private bool _snapping;
        [FormerlySerializedAs("fadeOut")] [SerializeField] private bool _fadeOut = true;
        
        [Header("Ease")]
        [FormerlySerializedAs("ease")] [SerializeField] private Ease _ease = Ease.OutQuad;

        private Tween activeShakeTween;
        
        public static Action TriggerShake;

        private void Awake()
        {
            if (_targetToShake == null) 
                _targetToShake = transform;
        }

        private void OnEnable() => 
            TriggerShake += PlayShake;

        private void OnDisable() => 
            TriggerShake -= PlayShake;

        public void PlayShake()
        {
            if (_targetToShake == null)
                return;

            if (activeShakeTween != null && activeShakeTween.IsActive()) 
                activeShakeTween.Kill(true);

            activeShakeTween = _targetToShake
                .DOShakePosition(_durationSeconds, _strength, _vibrato, _randomness, _snapping, _fadeOut)
                .SetEase(_ease);
        }
    }
}


