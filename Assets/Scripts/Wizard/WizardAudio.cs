using UnityEngine;
using DG.Tweening;

namespace Wizard
{
    public class WizardAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _fadeDuration = 2f;
        
        private float _initialVolume = 1f;
        private Tween _fadeTween;
        
        private void Awake()
        {
            if (_audioSource)
                _initialVolume = _audioSource.volume;
        }

        public void Initialize(WizardSlide slide)
        {
            // cancel fade if a new track is about to start
            if (_fadeTween != null && _fadeTween.IsActive())
            {
                _fadeTween.Kill();
                _fadeTween = null;
                _audioSource.Stop();
            }

            if (!_audioSource || !slide.AudioClip)
                return;
            
 
            _audioSource.volume = _initialVolume;
            _audioSource.PlayOneShot(slide.AudioClip);
        }
        
        public void Stop()
        {
            if (!_audioSource)
                return;
            
            if (_fadeTween != null && _fadeTween.IsActive())
            {
                _fadeTween.Kill();
                _fadeTween = null;
            }

            _fadeTween = DOTween
                .To(() => _audioSource.volume, v => _audioSource.volume = v, 0f, _fadeDuration)
                .OnComplete(() =>
                {
                    _audioSource.Stop();
                    _audioSource.volume = _initialVolume;
                    _fadeTween = null;
                });
        }
    }
}
