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

            // Add new text letter by letter
            // If the next two chars are "\n" (backslash + n), add both at once
            {
                int i = 1;
                while (i <= newText.Length)
                {
                    int targetIndex = i;
                    if (i <= newText.Length - 1)
                    {
                        int currentCharIndex = i - 1;
                        int nextCharIndex = i;
                        if (newText[currentCharIndex] == '\\' && newText[nextCharIndex] == 'n')
                        {
                            targetIndex = i + 1;
                        }
                    }

                    int index = targetIndex; // capture
                    seq.AppendCallback(() =>
                    {
                        _text.text = newText.Substring(0, index);
                    });
                    if (index < newText.Length)
                    {
                        seq.AppendInterval(letterInterval);
                    }

                    i = index + 1;
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
