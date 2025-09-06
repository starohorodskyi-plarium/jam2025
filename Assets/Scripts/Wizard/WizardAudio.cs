using UnityEngine;

namespace Wizard
{
    public class WizardAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _pitch;
        
        public void Initialize(WizardSlide slide)
        {
            if (!_audioSource || !slide.AudioClip)
                return;
            
            _audioSource.PlayOneShot(slide.AudioClip);
        }
    }
}
