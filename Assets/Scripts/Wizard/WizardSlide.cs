using UnityEngine;

namespace Wizard
{
    [CreateAssetMenu(menuName = "Wizard/Slide")]
    public class WizardSlide : ScriptableObject
    {
        public WizardId WizardId;
        public WizardActorId ActorId;
        public string Text;
        public AudioClip AudioClip;
        public float Delay;

        public bool DisableInputs;
    }
}
