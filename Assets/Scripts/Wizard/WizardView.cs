using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Wizard
{
    public class WizardView : MonoBehaviour
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _textContainer;
        [SerializeField] private float letterInterval = 0.03f;
        
        private Sequence _sequence;
        
        public void Show(WizardSlide slide)
        {
            UpdateGoalMessageText(slide.Text);
            _textContainer.text = slide.Text;
            _container.SetActive(true);
        }

        private void UpdateGoalMessageText(string message)
        {
            if (!_text)
            {
                return;
            }

            _sequence?.Kill();

            string oldText = _text.text ?? string.Empty;
            string newText = message ?? string.Empty;

            if (Mathf.Approximately(letterInterval, 0f))
            {
                _text.text = newText;
                return;
            }

            var seq = DOTween.Sequence();

            // Remove old text letter by letter
            for (int i = oldText.Length; i >= 0; i--)
            {
                int index = i; // capture
                seq.AppendCallback(() =>
                {
                    _text.text = oldText.Substring(0, index);
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
                    _text.text = newText.Substring(0, index);
                });
                if (i < newText.Length)
                {
                    seq.AppendInterval(letterInterval);
                }
            }

            // Ensure final state is the full new text
            seq.OnComplete(() => _text.text = newText);

            _sequence = seq;
        }


        public void Hide()
        {
            _container.SetActive(false);
            _text.text = string.Empty;
        }
    }
}
