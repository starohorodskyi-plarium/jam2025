using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gun
{
    public class Recoil : MonoBehaviour
    {
        [Header("Target")]
        [FormerlySerializedAs("targetToRecoil")] [SerializeField] private Transform _targetToRecoil;
        
        [Header("Recoil Settings")] 
        [FormerlySerializedAs("recoilAngleDegrees")] [SerializeField, Range(0.1f, 45f)] private float _recoilAngleDegrees = 6f;
        [FormerlySerializedAs("durationSeconds")] [SerializeField, Min(0.01f)] private float _durationSeconds = 0.12f;
        [FormerlySerializedAs("vibrato")] [SerializeField, Min(1)] private int _vibrato = 12;
        [FormerlySerializedAs("elasticity")] [SerializeField, Range(0f, 1f)] private float _elasticity = 0.5f;
        
        [Header("Ease")] 
        [FormerlySerializedAs("ease")] [SerializeField] private Ease _ease = Ease.OutQuad;

        private Tween activeRecoilTween;

        private void Awake()
        {
            if (_targetToRecoil == null) 
                _targetToRecoil = transform;
        }

        public void PlayRecoil()
        {
            if (_targetToRecoil == null)
                return;

            if (activeRecoilTween != null && activeRecoilTween.IsActive()) 
                activeRecoilTween.Kill(true);

            var punch = new Vector3(-Mathf.Abs(_recoilAngleDegrees), 0f, 0f);
            activeRecoilTween = _targetToRecoil
                .DOPunchRotation(punch, _durationSeconds, _vibrato, _elasticity)
                .SetEase(_ease);
        }
    }
}
