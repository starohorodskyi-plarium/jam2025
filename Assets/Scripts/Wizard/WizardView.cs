using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Wizard
{
    public class WizardView : MonoBehaviour
    {
        [SerializeField] private GameObject _inspectorContainer;
        [SerializeField] private GameObject _crowdContainer;
        [SerializeField] private Transform _liveInspector;
        [SerializeField] private Transform _deadInspector;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _textContainer;

        [SerializeField] private TMP_Text _crowdText;
        [SerializeField] private TMP_Text _crowdTextContainer;
        
        [SerializeField] private float letterInterval = 0.03f;
        
        private Sequence _sequence;
        
        public void Show(WizardSlide slide)
        {
            var text = slide.ActorId == WizardActorId.Crowd
                ? _crowdText
                : _text;
            
            UpdateGoalMessageText(slide.Text, text);
            
            if (slide.ActorId == WizardActorId.Crowd)
            {
                _crowdContainer.SetActive(true);
                _crowdTextContainer.text = slide.Text;
            }
            else
            {
                _inspectorContainer.SetActive(true);
                _textContainer.text = slide.Text;
            }

            _liveInspector.gameObject.SetActive(slide.ActorId == WizardActorId.Live_Inspector);
            _deadInspector.gameObject.SetActive(slide.ActorId == WizardActorId.Dead_Inspector);
        }

        private void UpdateGoalMessageText(string message, TMP_Text text)
        {
            if (!text)
            {
                return;
            }

            _sequence?.Kill();

            string oldText = text.text ?? string.Empty;
            string newText = message ?? string.Empty;

            if (Mathf.Approximately(letterInterval, 0f))
            {
                text.text = newText;
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
                        text.text = newText.Substring(0, index);
                    });
                    if (index < newText.Length)
                    {
                        seq.AppendInterval(letterInterval);
                    }

                    i = index + 1;
                }
            }

            // Ensure final state is the full new text
            seq.OnComplete(() => text.text = newText);

            _sequence = seq;
        }


        public void Hide()
        {
            _inspectorContainer.SetActive(false);
        }
    }
}
