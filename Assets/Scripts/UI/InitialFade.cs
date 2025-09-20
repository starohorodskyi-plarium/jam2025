using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class InitialFade : MonoBehaviour
    {
        [FormerlySerializedAs("initialFadeUI")] [SerializeField] private Transform _initialFadeUI;
        [FormerlySerializedAs("initialFadeUICenter")] [SerializeField] private Transform _initialFadeUICenter;
        [FormerlySerializedAs("initialFadeUIInnedDarkness")] [SerializeField] private Transform _initialFadeUIInnedDarkness;
        [FormerlySerializedAs("animationDuration")] [SerializeField] private float _animationDuration = 1.5f;
        [FormerlySerializedAs("scaleUpFactor")] [SerializeField] private float _scaleUpFactor = 1.1f;

        private void Awake()
        {
            if (GameManager.Attempt	> 0)   
                _initialFadeUI.gameObject.SetActive(false);
        }

        private void Start()
        {
            if (_initialFadeUI.gameObject.activeSelf)
                ShowAnimation();
        }

        private void ShowAnimation()
        {
            if (_initialFadeUI == null)
                return;

            var fadeDuration = Mathf.Max(0f, _animationDuration * 0.1f);

            // Ensure the root is active
            _initialFadeUI.gameObject.SetActive(true);

            var sequence = DOTween.Sequence();

            // Scale up main UI over the full duration
            var startScale = _initialFadeUI.localScale;
            var targetScale = startScale * _scaleUpFactor;
            Tween scaleTween = _initialFadeUI.DOScale(targetScale, _animationDuration);
            sequence.Join(scaleTween);

            // Fade out inner darkness in 10% of total duration, in parallel
            if (_initialFadeUIInnedDarkness != null)
            {
                var fadeTween = CreateFadeOutTween(_initialFadeUIInnedDarkness, fadeDuration);
                if (fadeTween != null)
                    sequence.Join(fadeTween);
            }

            // Fade out center over the last 20% of the total duration
            if (_initialFadeUICenter != null)
            {
                var centerFadeStart = Mathf.Max(0f, _animationDuration * 0.8f);
                var centerFadeDuration = Mathf.Max(0f, _animationDuration * 0.2f);
                
                var centerFadeTween = CreateFadeOutTween(_initialFadeUICenter, centerFadeDuration);
                
                if (centerFadeTween != null)
                    sequence.Insert(centerFadeStart, centerFadeTween);
            }

            // Disable the root object after the whole sequence
            sequence.OnComplete(() =>
            {
                if (_initialFadeUI != null)
                    _initialFadeUI.gameObject.SetActive(false);
            });
        }

        private static Tween CreateFadeOutTween(Transform target, float duration)
        {
            if (target == null)
                return null;

            // Prefer CanvasGroup if present
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                return canvasGroup.DOFade(0f, duration);

            // Try UI Graphic (e.g., Image, Text)
            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
                return graphic.DOFade(0f, duration);

            // Try SpriteRenderer
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                return spriteRenderer.DOFade(0f, duration);

            // Try in children as a fallback
            var childGraphic = target.GetComponentInChildren<Graphic>(true);
            if (childGraphic != null)
                return childGraphic.DOFade(0f, duration);

            var childSprite = target.GetComponentInChildren<SpriteRenderer>(true);
            if (childSprite != null)
                return childSprite.DOFade(0f, duration);

            Debug.LogWarning($"InitialFade: No fade-capable component found on '{target.name}'. Add a CanvasGroup, UI Graphic, or SpriteRenderer.");
            return null;
        }
    }
}
