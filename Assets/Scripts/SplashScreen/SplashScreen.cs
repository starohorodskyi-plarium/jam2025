using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SplashScreen
{
    public class SplashScreen : MonoBehaviour
    {
	    [FormerlySerializedAs("parentPlatformScaler")] [SerializeField] private Transform _parentPlatformScaler;
	    [FormerlySerializedAs("desktopScale")] [SerializeField] private Vector3 _desktopScale;
	    [FormerlySerializedAs("mobileScale")] [SerializeField] private Vector3 _mobileScale;
	    [Space]
	    [Header("Audio")]
	    [FormerlySerializedAs("initialSound")] [SerializeField] private AudioSource _initialSound;
	    [FormerlySerializedAs("electroSound")] [SerializeField] private AudioSource _electroSound;
	    [FormerlySerializedAs("intenseElectroSound")] [SerializeField] private AudioSource _intenseElectroSound;
	    [FormerlySerializedAs("gearsSound")] [SerializeField] private AudioSource _gearsSound;
	    [FormerlySerializedAs("activation")] [SerializeField] private AudioSource _activation;
	    [Space]
	    [FormerlySerializedAs("activationParticles")] [SerializeField] private ParticleSystem _activationParticles;
        [Space]
        [FormerlySerializedAs("startMenuSceneName")] [SerializeField] private string _startMenuSceneName;
        [FormerlySerializedAs("sceneLoader")] [SerializeField] private UnityEvent<string> _sceneLoader;

        private void Awake()
        {
	#if UNITY_IOS || UNITY_ANDROID
	        _parentPlatformScaler.localScale = _mobileScale;
	#else
			_parentPlatformScaler.localScale = _desktopScale;
	#endif
        }

		public void LoadGameScene() =>
			_sceneLoader?.Invoke(_startMenuSceneName);

		public void ActivateLogo() =>
			_activationParticles.Play();
		
		public void PlayActivationSound() =>
			_activation.Play();
		
		public void PlayInitialSound() =>
			_initialSound.Play();
		
		public void PlayElectroSound() =>
			_electroSound.Play();
		
		public void StopElectroSound() =>
			_electroSound.Stop();
		
		public void PlayIntenseElectroSound() =>
			_intenseElectroSound.Play();
		
		public void StopIntenseElectroSound() =>
			_intenseElectroSound.Stop();
			
		public void PlayGearSound() =>
			_gearsSound.Play();
    }
}

