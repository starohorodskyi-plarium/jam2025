using UnityEngine;
using UnityEngine.Events;

namespace Wizard
{
    public class WizardListener : MonoBehaviour
    {
        [SerializeField] private WizardId _listenedWizardId;

        public UnityEvent OnWizardStarted;
        public UnityEvent OnWizardFinished;

        private WizardController _wizardController;

        private void Awake() => 
            _wizardController = FindFirstObjectByType<WizardController>();

        private void OnEnable()
        {
            _wizardController.OnWizardStarted += WizardStarted;
            _wizardController.OnWizardFinished += WizardFinished;
        }

        private void OnDisable()
        {
            _wizardController.OnWizardStarted -= WizardStarted;
            _wizardController.OnWizardFinished -= WizardFinished;
        }

        private void WizardStarted(WizardId wizardId)
        {
            if (_listenedWizardId == wizardId) 
                OnWizardStarted?.Invoke();
        }
        
        private void WizardFinished(WizardId wizardId)
        {
            if (_listenedWizardId == wizardId)
                OnWizardFinished?.Invoke();
        }

    }
}
