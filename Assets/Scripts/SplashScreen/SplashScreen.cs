using UnityEngine;
using UnityEngine.Events;

namespace SplashScreen
{
    public class SplashScreen : MonoBehaviour
    {
	    [SerializeField] private Transform parentPlatformScaler;
	    [SerializeField] private Vector3 desktopScale;
	    [SerializeField] private Vector3 mobileScale;
	    [Space]
	    [Header("Audio")]
	    [SerializeField] private AudioSource initialSound;
	    [SerializeField] private AudioSource electroSound;
	    [SerializeField] private AudioSource intenseElectroSound;
	    [SerializeField] private AudioSource gearsSound;
	    [SerializeField] private AudioSource activation;
	    [Space]
	    [SerializeField] private ParticleSystem activationParticles;
        [Space]
        [SerializeField] private string startMenuSceneName;
        [SerializeField] private UnityEvent<string> sceneLoader;

        private void Awake()
        {
	#if UNITY_IOS || UNITY_ANDROID
	        parentPlatformScaler.localScale = mobileScale;
	#else
			parentPlatformScaler.localScale = desktopScale;
	#endif
        }

		public void LoadGameScene() =>
			sceneLoader?.Invoke(startMenuSceneName);

		public void ActivateLogo() =>
			activationParticles.Play();
		
		public void PlayActivationSound() =>
			activation.Play();
		
		public void PlayInitialSound() =>
			initialSound.Play();
		
		public void PlayElectroSound() =>
			electroSound.Play();
		
		public void StopElectroSound() =>
			electroSound.Stop();
		
		public void PlayIntenseElectroSound() =>
			intenseElectroSound.Play();
		
		public void StopIntenseElectroSound() =>
			intenseElectroSound.Stop();
			
		public void PlayGearSound() =>
			gearsSound.Play();
    }
}

