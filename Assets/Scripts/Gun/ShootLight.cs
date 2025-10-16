using System;
using Core;
using UnityEngine;
using DG.Tweening;

namespace Gun
{
    public class ShootLight : MonoBehaviour
    {
        [SerializeField] private Light _light;

        [Header("Blink Settings")] 
        [SerializeField] private IntFloatClipDictionary _peakIntensity; 
        [SerializeField] private float _upDuration = 0.05f; 
        [SerializeField] private float _downDuration = 0.1f; 
        [SerializeField] private Ease _upEase = Ease.OutQuad; 
        [SerializeField] private Ease _downEase = Ease.InQuad; 
        [SerializeField] private bool _unscaledTime; 

        private Sequence _blinkSequence;

        private void OnEnable() => 
            _light.intensity = 0f;

        private void OnDisable()
        {
            _blinkSequence?.Kill();
            _light.intensity = 0f; 
        }
            

        public void Blink()
        {
            if (!_light) 
                return;

            _blinkSequence?.Kill();

            _light.intensity = 0f; 

            _blinkSequence = DOTween.Sequence();
            if (_unscaledTime)
                _blinkSequence.SetUpdate(true);

            _blinkSequence
                .Append(DOTween.To(() => _light.intensity, v => _light.intensity = v, _peakIntensity[GameManager.Instance.LoadedLevelId ?? 0], _upDuration).SetEase(_upEase))
                .Append(DOTween.To(() => _light.intensity, v => _light.intensity = v, 0f, _downDuration).SetEase(_downEase));
        }
        
            
        [Serializable]
        public class IntFloatClipDictionary : SerializableDictionary<int, float> 
        {
        }
    }
}
