using UnityEngine;

namespace Wizard
{
    [CreateAssetMenu(menuName = "Wizard/Slide")]
    public class WizardSlide : ScriptableObject
    {
        public WizardId WizardId;
        public string Text;
        public AudioClip AudioClip;

        public bool DisableInputs;
    }
}
