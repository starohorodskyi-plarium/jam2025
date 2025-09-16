using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InitialFade : MonoBehaviour
{
    [SerializeField] private Transform initialFadeUI;
    [SerializeField] private Transform initialFadeUICenter;
    [SerializeField] private Transform initialFadeUIInnedDarkness;
    [SerializeField] private float animationDuration = 1.5f;
    [SerializeField] private float scaleUpFactor = 1.1f;

    private void Awake()
    {
        if (GameManager.Attempt	> 0)   
            initialFadeUI.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (initialFadeUI.gameObject.activeSelf)
            ShowAnimation();
    }

    private void ShowAnimation()
    {
        if (initialFadeUI == null)
            return;

        float fadeDuration = Mathf.Max(0f, animationDuration * 0.1f);

        // Ensure the root is active
        initialFadeUI.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        // Scale up main UI over the full duration
        Vector3 startScale = initialFadeUI.localScale;
        Vector3 targetScale = startScale * scaleUpFactor;
        Tween scaleTween = initialFadeUI.DOScale(targetScale, animationDuration);
        sequence.Join(scaleTween);

        // Fade out inner darkness in 10% of total duration, in parallel
        if (initialFadeUIInnedDarkness != null)
        {
            Tween fadeTween = CreateFadeOutTween(initialFadeUIInnedDarkness, fadeDuration);
            if (fadeTween != null)
                sequence.Join(fadeTween);
        }

        // Fade out center over the last 20% of the total duration
        if (initialFadeUICenter != null)
        {
            float centerFadeStart = Mathf.Max(0f, animationDuration * 0.8f);
            float centerFadeDuration = Mathf.Max(0f, animationDuration * 0.2f);
            Tween centerFadeTween = CreateFadeOutTween(initialFadeUICenter, centerFadeDuration);
            if (centerFadeTween != null)
                sequence.Insert(centerFadeStart, centerFadeTween);
        }

        // Disable the root object after the whole sequence
        sequence.OnComplete(() =>
        {
            if (initialFadeUI != null)
                initialFadeUI.gameObject.SetActive(false);
        });
    }

    private Tween CreateFadeOutTween(Transform target, float duration)
    {
        if (target == null)
            return null;

        // Prefer CanvasGroup if present
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            return canvasGroup.DOFade(0f, duration);

        // Try UI Graphic (e.g., Image, Text)
        Graphic graphic = target.GetComponent<Graphic>();
        if (graphic != null)
            return graphic.DOFade(0f, duration);

        // Try SpriteRenderer
        SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            return spriteRenderer.DOFade(0f, duration);

        // Try in children as a fallback
        Graphic childGraphic = target.GetComponentInChildren<Graphic>(true);
        if (childGraphic != null)
            return childGraphic.DOFade(0f, duration);

        SpriteRenderer childSprite = target.GetComponentInChildren<SpriteRenderer>(true);
        if (childSprite != null)
            return childSprite.DOFade(0f, duration);

        Debug.LogWarning($"InitialFade: No fade-capable component found on '{target.name}'. Add a CanvasGroup, UI Graphic, or SpriteRenderer.");
        return null;
    }
}
