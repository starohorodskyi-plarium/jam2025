using UnityEngine;

namespace Wizard
{
    public class TriggerNextWizardSlide : MonoBehaviour
    {
        [SerializeField] private WizardId _wizardId;
        [SerializeField] private bool _showForSeconds;
        [SerializeField] private float _duration;

        public void TriggerCertain()
        {
            var wizardController = FindFirstObjectByType<WizardController>();
            wizardController.Show(_wizardId, _showForSeconds ? _duration : null);
        }
    }
}
