using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace UI.Goal
{
    public class GoalMessage : MonoBehaviour
    {
        [FormerlySerializedAs("goalMessageText")] [SerializeField] private TextMeshProUGUI _goalMessageText;
        [FormerlySerializedAs("letterInterval")] [SerializeField] private float _letterInterval = 0.03f;
        
        public static Action<string> UpdateGoalMessage;

        private Sequence _sequence;

        private void OnEnable() => 
            UpdateGoalMessage += UpdateGoalMessageText;

        private void UpdateGoalMessageText(string message)
        {
            if (_goalMessageText == null)
                return;

            _sequence?.Kill();

            var oldText = _goalMessageText.text ?? string.Empty;
            var newText = message ?? string.Empty;

            if (Mathf.Approximately(_letterInterval, 0f))
            {
                _goalMessageText.text = newText;
                return;
            }

            var seq = DOTween.Sequence();

            // Remove old text letter by letter
            for (var i = oldText.Length; i >= 0; i--)
            {
                var index = i; // capture
                seq.AppendCallback(() =>
                {
                    _goalMessageText.text = oldText[..index];
                });
                
                if (i > 0) 
                    seq.AppendInterval(_letterInterval);
            }

            // Add new text letter by letter
            for (var i = 1; i <= newText.Length; i++)
            {
                var index = i; // capture
                seq.AppendCallback(() =>
                {
                    _goalMessageText.text = newText[..index];
                });
                if (i < newText.Length) 
                    seq.AppendInterval(_letterInterval);
            }

            // Ensure final state is the full new text
            seq.OnComplete(() => _goalMessageText.text = newText);

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