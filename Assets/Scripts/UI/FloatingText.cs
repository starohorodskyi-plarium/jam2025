using System;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private float moveUpDistance = 20f;  // How far up the text moves
    private float duration = 0.5f;         // How long the effect lasts

    private TextMeshProUGUI text;
    private RectTransform rectTransform;
    private Color startColor;
    private Vector2 startPos;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        startColor = text.color;
    }

    private void OnEnable()
    {
        // Reset to initial state
        text.color = startColor;
        startPos = rectTransform.anchoredPosition;

        // Start the floating effect
        StartCoroutine(AnimateText());
    }

    private void OnDisable()
    {
        rectTransform.anchoredPosition = startPos;
    }

    private System.Collections.IEnumerator AnimateText()
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, moveUpDistance);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move upward
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            // Fade out
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }

        // After animation, hide the object
        gameObject.SetActive(false);
    }
}
