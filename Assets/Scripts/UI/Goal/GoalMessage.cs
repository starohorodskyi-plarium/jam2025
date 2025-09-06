using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UI.Goal
{
    public class GoalMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goalMessageText;
        [SerializeField] private float letterInterval = 0.03f;
        
        public static Action<string> UpdateGoalMessage;

        private Sequence _sequence;

        private void OnEnable()
        {
            UpdateGoalMessage += UpdateGoalMessageText;
        }

        private void UpdateGoalMessageText(string message)
        {
            if (goalMessageText == null)
            {
                return;
            }

            _sequence?.Kill();

            string oldText = goalMessageText.text ?? string.Empty;
            string newText = message ?? string.Empty;

            if (Mathf.Approximately(letterInterval, 0f))
            {
                goalMessageText.text = newText;
                return;
            }

            var seq = DOTween.Sequence();

            // Remove old text letter by letter
            for (int i = oldText.Length; i >= 0; i--)
            {
                int index = i; // capture
                seq.AppendCallback(() =>
                {
                    goalMessageText.text = oldText.Substring(0, index);
                });
                if (i > 0)
                {
                    seq.AppendInterval(letterInterval);
                }
            }

            // Add new text letter by letter
            for (int i = 1; i <= newText.Length; i++)
            {
                int index = i; // capture
                seq.AppendCallback(() =>
                {
                    goalMessageText.text = newText.Substring(0, index);
                });
                if (i < newText.Length)
                {
                    seq.AppendInterval(letterInterval);
                }
            }

            // Ensure final state is the full new text
            seq.OnComplete(() => goalMessageText.text = newText);

            _sequence = seq;
        }

        private void OnDisable()
        {
            UpdateGoalMessage -= UpdateGoalMessageText;
            _sequence?.Kill();
            _sequence = null;
        }
    }
}