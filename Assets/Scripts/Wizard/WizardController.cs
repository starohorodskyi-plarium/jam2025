using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wizard
{
    public class WizardController : MonoBehaviour
    {
        [SerializeField] private WizardView _view;
        [SerializeField] private List<WizardSlide> _slides;

        private WizardId _current;
        private WizardId _next;
        
        private bool _autoClose;
        private WizardSlide _currentSlide;

        public event Action<WizardId> OnWizardStarted;
        public event Action<WizardId> OnWizardFinished;

        private void OnEnable()
        {
            
        }

        public void ShowNext(float? forSeconds = null) => 
            Show(_next, forSeconds);
        
        public void Show(WizardId wizardId, float? forSeconds = null)
        {
            var slide = _slides.FirstOrDefault(x => x.WizardId == wizardId);
            if (slide == null)
                return;
            
            if (slide.DisableInputs)
                GameManager.Instance.DisableInputs();
            
            _view.Show(slide);
            _current = wizardId;
            _currentSlide = slide;

            _autoClose = forSeconds.HasValue;
            
            if (forSeconds.HasValue)
                StartCoroutine(ShowRoutine(forSeconds.Value));
            
            OnWizardStarted?.Invoke(wizardId);
        }

        public void Hide()
        {
            if (_currentSlide == null)
                return;
            
            if (_currentSlide.DisableInputs)
                GameManager.Instance.EnableInputs();

            var index = _current;

            _current = default;
            _currentSlide = default;
            _autoClose = default;
            _view.Hide();
            
            OnWizardFinished?.Invoke(index);
        }
        
        private void Update()
        {
            if (_autoClose)
                return;
            
#if ENABLE_INPUT_SYSTEM
            var pressed = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                          || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
#else
        bool pressed = Input.GetMouseButtonDown(0);
        if (!pressed && Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    pressed = true;
                    break;
                }
            }
        }
#endif

            if (pressed) 
                Hide();
        }

        private IEnumerator ShowRoutine(float durationSeconds)
        {
            yield return new WaitForSecondsRealtime(durationSeconds);
            Hide();
        }
    }
}
