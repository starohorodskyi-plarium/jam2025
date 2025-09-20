using TMPro;
using UnityEngine;

namespace UI
{
    public class FloatingText : MonoBehaviour
    {
        private const float MoveUpDistance = 20f; // How far up the text moves
        private const float Duration = 0.5f; // How long the effect lasts

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

        private void OnDisable() => 
            rectTransform.anchoredPosition = startPos;

        private System.Collections.IEnumerator AnimateText()
        {
            var elapsed = 0f;
            var anchoredPosition = rectTransform.anchoredPosition;
            var endPos = anchoredPosition + new Vector2(0, MoveUpDistance);

            while (elapsed < Duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / Duration;

                // Move upward
                rectTransform.anchoredPosition = Vector2.Lerp(anchoredPosition, endPos, t);

                // Fade out
                text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

                yield return null;
            }

            // After animation, hide the object
            gameObject.SetActive(false);
        }
    }
}
