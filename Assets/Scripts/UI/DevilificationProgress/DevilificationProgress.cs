using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class DevilificationProgress : MonoBehaviour
{
    [Header("Slider Reference")]
    [SerializeField] private Slider _slider;

    [Header("Tween Settings")]
    [SerializeField] private float _tweenDuration = 0.5f;
    [SerializeField] private Ease _tweenEase = Ease.OutQuad;
    [SerializeField] private bool _ignoreTimeScale = false;

    private float _minValue;
    private float _maxValue;
    private Tween _activeTween;

    public static float CurrentValue { get; private set; }

    public static Action<float> OnSetInstant;
    public static Action<float> OnSetSmooth;

    private void Awake()
    {
        if (_slider == null)
        {
            _slider = GetComponent<Slider>();
        }

        if (_slider == null)
        {
            Debug.LogError("DevilificationProgress: Slider reference is missing.");
            return;
        }

        _minValue = _slider.minValue;
        _maxValue = _slider.maxValue;
        CurrentValue = _slider.value;
    }

    private void OnEnable()
    {
        OnSetInstant += HandleSetInstant;
        OnSetSmooth += HandleSetSmooth;
    }

    private void OnDisable()
    {
        OnSetInstant -= HandleSetInstant;
        OnSetSmooth -= HandleSetSmooth;
    }

    private void OnDestroy()
    {
        if (_activeTween != null && _activeTween.IsActive())
        {
            _activeTween.Kill();
            _activeTween = null;
        }
    }

    private void HandleSetInstant(float newValue)
    {
        if (_slider == null) return;

        if (_activeTween != null && _activeTween.IsActive())
        {
            _activeTween.Kill();
            _activeTween = null;
        }

        float clamped = Mathf.Clamp(newValue, _minValue, _maxValue);
        _slider.value = clamped;
        CurrentValue = clamped;
    }

    private void HandleSetSmooth(float targetValue)
    {
        if (_slider == null) return;

        float clampedTarget = Mathf.Clamp(targetValue, _minValue, _maxValue);

        if (_activeTween != null && _activeTween.IsActive())
        {
            _activeTween.Kill();
        }

        _activeTween = DOTween
            .To(() => _slider.value, x =>
            {
                _slider.value = x;
                CurrentValue = x;
            }, clampedTarget, _tweenDuration)
            .SetEase(_tweenEase)
            .SetUpdate(_ignoreTimeScale);
    }
}
