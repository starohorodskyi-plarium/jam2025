using Solo.MOST_IN_ONE;
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
        public Most_HapticFeedback.HapticTypes HapticType = Most_HapticFeedback.HapticTypes.None;
        public float Delay;

        public bool DisableInputs;
    }
}
