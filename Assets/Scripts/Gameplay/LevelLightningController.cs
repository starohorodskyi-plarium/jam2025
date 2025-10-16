using Gun;
using Lighting;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Gameplay
{
    public class Level2Lightning : MonoBehaviour
    {
        [SerializeField] private WeaponLightning _weapon;
        [SerializeField] private Level2CitizensLightController _citizensLightController;
        
        [Header("Thunderstorm")]
        [SerializeField] private Light _thunderLight;
        [SerializeField] private AudioSource _thunderSound;
        [SerializeField] private Vector2 _lightningIntervalRange = new Vector2(5f, 15f);
        [SerializeField] private float _thunderSoundDelay = 1f;
        [SerializeField] private Vector2 _thunderPitchRange = new Vector2(0.9f, 1.1f);
        [SerializeField] private float _lightningPeakIntensity = 8f;
        [SerializeField] private float _lightningFlickerDuration = 0.3f;

        private Coroutine _thunderstormCoroutine;

        private static readonly Level2CitizensLightController.LightningSource ShootLight = new()
        {
            Position = new Vector3(0,1.64f,0),
            Color = new Color(1f,0.89f,0.83f),
            Intensity = 1f,
            Range = 45f,
        };
        
        private static readonly Level2CitizensLightController.LightningSource TradingPostLight = new()
        {
            Position = new Vector3(-5,1.14f,18.95f),
            Color = new Color(0.44f,1f,0.71f),
            Intensity = 1f,
            Range = 7f,
        };
        
        private static readonly Level2CitizensLightController.LightningSource ThunderLight = new()
        {
            Position = new Vector3(0f,20f,-2f),
            Color = new Color(0.7f,0.86f,0.99f),
            Intensity = 2f,
            Range = 150f,
        };

        public void InitializeLightning()
        {
            enabled = true;
            _weapon.SetDarkModeInstant();
            _citizensLightController.ActivateCalculation();
            _citizensLightController.AddLightSource(TradingPostLight);
            
            if (_thunderLight != null)
            {
                _thunderLight.gameObject.SetActive(false);
                _thunderLight.intensity = 0f;
            }
            
            StartThunderstorm();
        }
        
        public void ResetLightning()
        {
            _weapon.SetLightModeInstant();
            _citizensLightController.DeactivateCalculation();
            StopThunderstorm();
            
            if (_thunderLight != null)
                _thunderLight.gameObject.SetActive(false);
            
            enabled = false;
        }

        public void ShootReaction()
        {
            if (!enabled) 
                return;
            
            _weapon.ShootReaction();
            _citizensLightController.AddTemporaryLightSource(ShootLight, 0.15f);
        }
        
        private void StartThunderstorm()
        {
            if (_thunderstormCoroutine != null)
                StopCoroutine(_thunderstormCoroutine);
            
            _thunderstormCoroutine = StartCoroutine(ThunderstormSequence());
        }
        
        private void StopThunderstorm()
        {
            if (_thunderstormCoroutine != null)
            {
                StopCoroutine(_thunderstormCoroutine);
                _thunderstormCoroutine = null;
            }
            
            DOTween.Kill(_thunderLight);
        }
        
        private IEnumerator ThunderstormSequence()
        {
            while (enabled)
            {
                float waitTime = Random.Range(_lightningIntervalRange.x, _lightningIntervalRange.y);
                yield return new WaitForSeconds(waitTime);
                
                TriggerLightning();
            }
        }
        
        private void TriggerLightning()
        {
            if (_thunderLight == null)
                return;
            
            _thunderLight.gameObject.SetActive(true);
            _thunderLight.intensity = 0f;
            
            DOTween.Kill(_thunderLight);
            
            _weapon.ThunderReaction();
            _citizensLightController.AddTemporaryLightSource(ThunderLight, 0.522f);
            
            Sequence lightningSequence = DOTween.Sequence();
            
            // Quick flash up
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity * 0.7f, 0.05f));
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity * 0.2f, 0.05f));
            
            // Main strike
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity, 0.08f));
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity * 0.4f, 0.06f));
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity * 0.9f, 0.04f));
            
            // Fade out with flicker
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity * 0.3f, 0.1f));
            lightningSequence.Append(_thunderLight.DOIntensity(_lightningPeakIntensity * 0.5f, 0.05f));
            lightningSequence.Append(_thunderLight.DOIntensity(0f, 0.15f));
            
            lightningSequence.OnComplete(() => _thunderLight.gameObject.SetActive(false));
            
            // Play thunder sound with delay
            if (_thunderSound != null)
            {
                StartCoroutine(PlayThunderWithDelay(_thunderSoundDelay));
            }
        }
        
        private IEnumerator PlayThunderWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (_thunderSound != null)
            {
                _thunderSound.pitch = Random.Range(_thunderPitchRange.x, _thunderPitchRange.y);
                _thunderSound.Play();
            }
        }
    }
}
