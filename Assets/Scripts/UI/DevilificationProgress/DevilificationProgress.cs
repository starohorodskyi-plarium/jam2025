using System;
using DG.Tweening;
using Gameplay.DevilMode;
using UnityEngine;

namespace UI.DevilificationProgress
{
    public class DevilificationProgress : MonoBehaviour
    {
        public const int DemonsStaticCount = 9;
        public static int KilledDemonsCount = 0;

        [Header("Slider Reference")]
        [SerializeField] private RectTransform _devilificationSliderTransform;
        [SerializeField] private RectTransform _hpSliderTransform;
        [SerializeField] private CanvasGroup _devilificationCanvasGroup;
        [SerializeField] private CanvasGroup _hpCanvasGroup;
        
        [Header("Slider Positions")]
        [SerializeField] private float _fullProgressPositionX = 0f;
        [SerializeField] private float _emptyProgressPositionX = -206f;

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

        private const float MinValue = 0f;
        private const float MaxValue = 1f;
        private Tween _activeTween;
        private Tween _activeColorTween;
        private Tween _activeMoveTween;

        public static Action<float> OnSetInstant;
        public static Action<float> OnSetSmooth;

        public static void ResetProgress() => 
            KilledDemonsCount = 0;

        private void Awake()
        {
            if (_rectTransform == null) 
                _rectTransform = GetComponent<RectTransform>();
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
            HandleSetSmooth(MinValue);
        }

        private void DevilModeActivate()
        {
            SetColorsToHPMode();
            MoveToPositionB();
            HandleSetInstant(MaxValue);
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
            if (_activeTween != null && _activeTween.IsActive())
            {
                _activeTween.Kill();
                _activeTween = null;
            }

            var clamped = Mathf.Clamp01(newValue);
            var positionX = Mathf.Lerp(_emptyProgressPositionX, _fullProgressPositionX, clamped);
            
            if (_devilificationSliderTransform != null)
            {
                var devilPos = _devilificationSliderTransform.anchoredPosition;
                devilPos.x = positionX;
                _devilificationSliderTransform.anchoredPosition = devilPos;
            }
            
            if (_hpSliderTransform != null)
            {
                var hpPos = _hpSliderTransform.anchoredPosition;
                hpPos.x = positionX;
                _hpSliderTransform.anchoredPosition = hpPos;
            }
        }

        private void HandleSetSmooth(float targetValue)
        {
            var clampedTarget = Mathf.Clamp01(targetValue);

            if (_activeTween != null && _activeTween.IsActive()) 
                _activeTween.Kill();

            var currentValue = 0f;
            if (_devilificationSliderTransform != null)
            {
                var currentX = _devilificationSliderTransform.anchoredPosition.x;
                currentValue = Mathf.InverseLerp(_emptyProgressPositionX, _fullProgressPositionX, currentX);
            }

            _activeTween = DOTween
                .To(() => currentValue, x =>
                {
                    var positionX = Mathf.Lerp(_emptyProgressPositionX, _fullProgressPositionX, x);
                    
                    if (_devilificationSliderTransform != null)
                    {
                        var devilPos = _devilificationSliderTransform.anchoredPosition;
                        devilPos.x = positionX;
                        _devilificationSliderTransform.anchoredPosition = devilPos;
                    }
                    
                    if (_hpSliderTransform != null)
                    {
                        var hpPos = _hpSliderTransform.anchoredPosition;
                        hpPos.x = positionX;
                        _hpSliderTransform.anchoredPosition = hpPos;
                    }
                    
                    currentValue = x;
                }, clampedTarget, _tweenDuration)
                .SetEase(_tweenEase)
                .SetUpdate(_ignoreTimeScale);
        }

        private void SetColorsToDevilificationMode()
        {
            if (_activeColorTween != null && _activeColorTween.IsActive())
            {
                _activeColorTween.Kill();
                _activeColorTween = null;
            }

            var sequence = DOTween.Sequence();
            
            if (_devilificationCanvasGroup != null) 
                sequence.Join(_devilificationCanvasGroup.DOFade(1f, _colorTweenDuration));
            
            if (_hpCanvasGroup != null) 
                sequence.Join(_hpCanvasGroup.DOFade(0f, _colorTweenDuration));

            _activeColorTween = sequence
                .SetEase(_colorTweenEase)
                .SetUpdate(_colorIgnoreTimeScale);
        }

        private void SetColorsToHPMode()
        {
            if (_activeColorTween != null && _activeColorTween.IsActive())
            {
                _activeColorTween.Kill();
                _activeColorTween = null;
            }

            var sequence = DOTween.Sequence();
            
            if (_devilificationCanvasGroup != null) 
                sequence.Join(_devilificationCanvasGroup.DOFade(0f, _colorTweenDuration));
            
            if (_hpCanvasGroup != null) 
                sequence.Join(_hpCanvasGroup.DOFade(1f, _colorTweenDuration));

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
