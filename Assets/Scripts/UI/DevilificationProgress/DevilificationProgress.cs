using System;
using DG.Tweening;
using Gameplay.DevilMode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.DevilificationProgress
{
    public class DevilificationProgress : MonoBehaviour
    {
        public const float DevilificationLevelId = 3f;

        [Header("Slider Reference")]
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _background;
        [SerializeField] private Image _sliderImage;

        [Header("Tween Settings")]
        [SerializeField] private float _tweenDuration = 0.5f;
        [SerializeField] private Ease _tweenEase = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale;
    
        [Header("Color Tween Settings")]
        [SerializeField] private float _colorTweenDuration = 0.35f;
        [SerializeField] private Ease _colorTweenEase = Ease.OutQuad;
        [SerializeField] private bool _colorIgnoreTimeScale;
    
        [Header("Move Tween Settings")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _moveTweenDuration = 0.4f;
        [SerializeField] private Ease _moveTweenEase = Ease.OutQuad;
        [SerializeField] private bool _moveIgnoreTimeScale;
        [SerializeField] private Vector2 _movePositionA;
        [SerializeField] private Vector2 _movePositionB;
        
        [Header("DevilificationSettings")]
        [FormerlySerializedAs("backgroundDevilificationColor")] [SerializeField] private Color _backgroundDevilificationColor ;
        [FormerlySerializedAs("sliderDevilificationColor")] [SerializeField] private Color _sliderDevilificationColor;
        
        [Header("HPSettings")]
        [FormerlySerializedAs("backgroundHPColor")] [SerializeField] private Color _backgroundHPColor ;
        [FormerlySerializedAs("sliderHPColor")] [SerializeField] private Color _sliderHPColor;

        private float _minValue;
        private float _maxValue;
        private Tween _activeTween;
        private Tween _activeColorTween;
        private Tween _activeMoveTween;

        public static Action<float> OnSetInstant;
        public static Action<float> OnSetSmooth;

        private void Awake()
        {
            if (_slider == null) 
                _slider = GetComponent<Slider>();

            if (_rectTransform == null) 
                _rectTransform = GetComponent<RectTransform>();

            if (_slider == null)
            {
                Debug.LogError("DevilificationProgress: Slider reference is missing.");
                return;
            }

            _minValue = _slider.minValue;
            _maxValue = _slider.maxValue;
        }

        private void OnEnable()
        {
            DevilModeScenario.DevilModeActivated += DevilModeActivate;
            DevilModeScenario.DevilModeDeactivated += DevilModeDeactivate;
            OnSetInstant += HandleSetInstant;
            OnSetSmooth += HandleSetSmooth;
        }

        private void OnDisable()
        {
            DevilModeScenario.DevilModeActivated -= DevilModeActivate;
            DevilModeScenario.DevilModeDeactivated -= DevilModeDeactivate;
            OnSetInstant -= HandleSetInstant;
            OnSetSmooth -= HandleSetSmooth;
        }
    
        private void DevilModeDeactivate()
        {
            SetColorsToDevilificationMode();
            MoveToPositionA();
            HandleSetSmooth(_minValue);
        }

        private void DevilModeActivate()
        {
            SetColorsToHPMode();
            MoveToPositionB();
            HandleSetSmooth(_maxValue);
        }

        private void OnDestroy()
        {
            if (_activeTween != null && _activeTween.IsActive())
            {
                _activeTween.Kill();
                _activeTween = null;
            }
        
            if (_activeColorTween != null && _activeColorTween.IsActive())
            {
                _activeColorTween.Kill();
                _activeColorTween = null;
            }

            if (_activeMoveTween == null || !_activeMoveTween.IsActive()) 
                return;
            
            _activeMoveTween.Kill();
            _activeMoveTween = null;
        }

        private void HandleSetInstant(float newValue)
        {
            if (_slider == null) 
                return;

            if (_activeTween != null && _activeTween.IsActive())
            {
                _activeTween.Kill();
                _activeTween = null;
            }

            var clamped = Mathf.Clamp(newValue, _minValue, _maxValue);
            _slider.value = clamped;
        }

        private void HandleSetSmooth(float targetValue)
        {
            if (_slider == null) 
                return;

            var clampedTarget = Mathf.Clamp(targetValue, _minValue, _maxValue);

            if (_activeTween != null && _activeTween.IsActive()) 
                _activeTween.Kill();

            _activeTween = DOTween
                .To(() => _slider.value, x =>
                {
                    _slider.value = x;
                }, clampedTarget, _tweenDuration)
                .SetEase(_tweenEase)
                .SetUpdate(_ignoreTimeScale);
        }

        private void SetColorsToDevilificationMode()
        {
            if (_background == null && _sliderImage == null) 
                return;

            if (_activeColorTween != null && _activeColorTween.IsActive())
            {
                _activeColorTween.Kill();
                _activeColorTween = null;
            }

            var sequence = DOTween.Sequence();
            
            if (_background != null) 
                sequence.Join(_background.DOColor(_backgroundDevilificationColor, _colorTweenDuration));
            
            if (_sliderImage != null) 
                sequence.Join(_sliderImage.DOColor(_sliderDevilificationColor, _colorTweenDuration));

            _activeColorTween = sequence
                .SetEase(_colorTweenEase)
                .SetUpdate(_colorIgnoreTimeScale);
        }

        private void SetColorsToHPMode()
        {
            if (_background == null && _sliderImage == null) 
                return;

            if (_activeColorTween != null && _activeColorTween.IsActive())
            {
                _activeColorTween.Kill();
                _activeColorTween = null;
            }

            var sequence = DOTween.Sequence();
            
            if (_background != null) 
                sequence.Join(_background.DOColor(_backgroundHPColor, _colorTweenDuration));
            if (_sliderImage != null) 
                sequence.Join(_sliderImage.DOColor(_sliderHPColor, _colorTweenDuration));

            _activeColorTween = sequence
                .SetEase(_colorTweenEase)
                .SetUpdate(_colorIgnoreTimeScale);
        }

        private void MoveToPositionA()
        {
            if (_rectTransform == null) 
                return;

            if (_activeMoveTween != null && _activeMoveTween.IsActive())
            {
                _activeMoveTween.Kill();
                _activeMoveTween = null;
            }

            _activeMoveTween = _rectTransform
                .DOAnchorPos(_movePositionA, _moveTweenDuration)
                .SetEase(_moveTweenEase)
                .SetUpdate(_moveIgnoreTimeScale);
        }

        private void MoveToPositionB()
        {
            if (_rectTransform == null) 
                return;

            if (_activeMoveTween != null && _activeMoveTween.IsActive())
            {
                _activeMoveTween.Kill();
                _activeMoveTween = null;
            }

            _activeMoveTween = _rectTransform
                .DOAnchorPos(_movePositionB, _moveTweenDuration)
                .SetEase(_moveTweenEase)
                .SetUpdate(_moveIgnoreTimeScale);
        }
    }
}
