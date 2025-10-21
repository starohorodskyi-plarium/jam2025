using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace Wizard
{
    public class WizardView : MonoBehaviour
    {
        private static readonly int IsTalking = Animator.StringToHash("IsTalking");
        
        [SerializeField] private GameObject _inspectorContainer;
        [SerializeField] private GameObject _crowdContainer;
        [SerializeField] private GameObject _newspapperContainer;
        [SerializeField] private Transform _liveInspector;
        [SerializeField] private Transform _deadInspector;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _textContainer;

        [SerializeField] private TMP_Text _crowdText;
        [SerializeField] private TMP_Text _crowdTextContainer;
        
        [SerializeField] private WizardAudio _voice;
        [SerializeField] private Animator _characterAnimator;

        [FormerlySerializedAs("letterInterval")] [SerializeField] private float _letterInterval = 0.03f;
        
        private Sequence _sequence;
        private Coroutine _showRoutine;
        private Coroutine _voiceDelayRoutine;
        
        public void Show(WizardSlide slide)
        {
            if (_showRoutine != null)
            {
                StopCoroutine(_showRoutine);
                _showRoutine = null;
            }

            _showRoutine = StartCoroutine(ShowRoutine(slide));
        }

        public IEnumerator ShowAsync(WizardSlide slide)
        {
            if (_showRoutine != null)
            {
                StopCoroutine(_showRoutine);
                _showRoutine = null;
            }

            _showRoutine = StartCoroutine(ShowRoutine(slide));
            yield return _showRoutine;
        }

        private IEnumerator ShowRoutine(WizardSlide slide)
        {
            if (slide && slide.Delay > 0f)
                yield return new WaitForSeconds(slide.Delay);

            _voice.Stop();
            
            if(_characterAnimator.gameObject.activeInHierarchy)
                _characterAnimator.SetBool(IsTalking, false);
            
            CancelVoiceDelay();

            _newspapperContainer.SetActive(slide.WizardId == WizardId.Introduction);
            _crowdContainer.SetActive(slide.ActorId == WizardActorId.Crowd);
            _inspectorContainer.SetActive(slide.ActorId == WizardActorId.Live_Inspector || slide.ActorId == WizardActorId.Dead_Inspector);
            
            _voice.Initialize(slide);
            if (slide && slide.AudioClip)
            {
                CancelVoiceDelay();
                _voiceDelayRoutine = StartCoroutine(DisableTalkingAfterRealtime(slide.AudioClip.length * 0.95f));
            }
            
            if (slide.WizardId == WizardId.Introduction)
                yield break;
            
            var text = slide.ActorId == WizardActorId.Crowd
                ? _crowdText
                : _text;
            
            UpdateGoalMessageText(slide.Text, text);
            
            var visibleText = RemoveSquareBrackets(slide.Text);
            if (slide.ActorId == WizardActorId.Crowd)
                _crowdTextContainer.text = visibleText;
            else
                _textContainer.text = visibleText;

            _liveInspector.gameObject.SetActive(slide.ActorId == WizardActorId.Live_Inspector);
            _deadInspector.gameObject.SetActive(slide.ActorId == WizardActorId.Dead_Inspector);

            yield return null;
            
            if(_characterAnimator.gameObject.activeInHierarchy)
                _characterAnimator.SetBool(IsTalking, true);
        }

        private void CancelVoiceDelay()
        {
            if (_voiceDelayRoutine != null)
            {
                StopCoroutine(_voiceDelayRoutine);
                _voiceDelayRoutine = null;
            }
        }

        private IEnumerator DisableTalkingAfterRealtime(float seconds)
        {
            if (seconds <= 0f)
            {
                if(_characterAnimator.gameObject.activeInHierarchy)
                    _characterAnimator.SetBool(IsTalking, false);
                
                _voiceDelayRoutine = null;
                yield break;
            }

            yield return new WaitForSecondsRealtime(seconds);
            
            if(_characterAnimator.gameObject.activeInHierarchy)
                _characterAnimator.SetBool(IsTalking, false);
            
            _voiceDelayRoutine = null;
        }

        private void UpdateGoalMessageText(string message, TMP_Text text)
        {
            if (!text)
                return;
            
            _sequence?.Kill();
            
            var newText = message ?? string.Empty;
            var finalText = RemoveSquareBrackets(newText);

            if (Mathf.Approximately(_letterInterval, 0f))
            {
                text.text = finalText;
                return;
            }

            var seq = DOTween.Sequence();

            var openIndex = newText.IndexOf('[');
            var closeIndex = openIndex >= 0 ? newText.IndexOf(']', openIndex + 1) : -1;
            var hasBracketChunk = openIndex >= 0 && closeIndex > openIndex;

            if (!hasBracketChunk)
            {
                // Default behavior: type full text letter-by-letter (using finalText)
                var i = 1;
                while (i <= finalText.Length)
                {
                    var targetIndex = i;
                    if (i <= finalText.Length - 1)
                    {
                        var currentCharIndex = i - 1;

                        if (finalText[currentCharIndex] == '\\' && finalText[i] == 'n') 
                            targetIndex = i + 1;
                    }

                    var index = targetIndex; // capture
                    seq.AppendCallback(() => { text.text = finalText.Substring(0, index); });
                    
                    if (index < finalText.Length) 
                        seq.AppendInterval(_letterInterval);

                    i = index + 1;
                }
            }
            else
            {
                var prefix = newText[..openIndex];
                var inner = newText.Substring(openIndex + 1, closeIndex - openIndex - 1);
                var suffix = newText[(closeIndex + 1)..];

                // Type prefix letter-by-letter
                var i = 1;
                while (i <= prefix.Length)
                {
                    var targetIndex = i;
                    if (i <= prefix.Length - 1)
                    {
                        var currentCharIndex = i - 1;
                        
                        if (prefix[currentCharIndex] == '\\' && prefix[i] == 'n') 
                            targetIndex = i + 1;
                    }

                    var index = targetIndex; // capture
                    seq.AppendCallback(() => { text.text = prefix[..index]; });
                    
                    if (index < prefix.Length) 
                        seq.AppendInterval(_letterInterval);

                    i = index + 1;
                }

                // Show entire bracketed content at once (without brackets)
                seq.AppendCallback(() => { text.text = prefix + inner; });
                
                if (suffix.Length > 0) 
                    seq.AppendInterval(_letterInterval);

                // Continue typing the suffix letter-by-letter
                i = 1;
                while (i <= suffix.Length)
                {
                    var targetIndex = i;
                    if (i <= suffix.Length - 1)
                    {
                        var currentCharIndex = i - 1;
                        
                        if (suffix[currentCharIndex] == '\\' && suffix[i] == 'n') 
                            targetIndex = i + 1;
                    }

                    var index = targetIndex; // capture
                    seq.AppendCallback(() => { text.text = prefix + inner + suffix[..index]; });
                    
                    if (index < suffix.Length) 
                        seq.AppendInterval(_letterInterval);

                    i = index + 1;
                }
            }

            // Ensure final state is the full new text without brackets
            seq.OnComplete(() =>
            {
                text.text = finalText;
            });

            _sequence = seq;
        }

        private static string RemoveSquareBrackets(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var openIndex = value.IndexOf('[');
            if (openIndex < 0)
                return value;

            var closeIndex = value.IndexOf(']', openIndex + 1);
            if (closeIndex < 0)
                return value;

            var withoutClose = value.Remove(closeIndex, 1);
            var withoutBoth = withoutClose.Remove(openIndex, 1);
            
            return withoutBoth;
        }

        public void Hide()
        {
            _crowdContainer.SetActive(false);
            _inspectorContainer.SetActive(false);
            _newspapperContainer.SetActive(false);
            
            _voice.Stop();
            CancelVoiceDelay();
            
            if(_characterAnimator.gameObject.activeInHierarchy)
                _characterAnimator.SetBool(IsTalking, false);
        }
    }
}
