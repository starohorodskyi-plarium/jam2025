using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class RandomDemonSounds : MonoBehaviour
    {
        [Header("Audio")] 
        [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource _audioSource;
        [FormerlySerializedAs("clips")] [SerializeField] private AudioClip[] _clips;
        
        [Header("Pitch Settings")] 
        [FormerlySerializedAs("minPitch")] [SerializeField] [Range(0.1f, 3f)] private float _minPitch = 0.9f;
        [FormerlySerializedAs("maxPitch")] [SerializeField] [Range(0.1f, 3f)] private float _maxPitch = 1.1f;
        
        [Header("Interval (seconds)")]
        [FormerlySerializedAs("minInterval")] [SerializeField] [Min(0f)] private float _minInterval = 6f;
        [FormerlySerializedAs("maxInterval")] [SerializeField] [Min(0f)] private float _maxInterval = 12f;

        private Coroutine playRoutine;
        private GameManager gameManager;
        private bool isLevelActive;

        private void Awake()
        {
            if (!_audioSource)
                _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            gameManager = FindFirstObjectByType<GameManager>();
        
            if (!gameManager) 
                return;
        
            gameManager.OnLevelStarted += HandleLevelStarted;
            gameManager.OnLevelFinishedFailed += HandleLevelEnded;
            gameManager.OnLevelFinishedSuccess += HandleLevelEnded;

            if (gameManager.CurrentState == GameManager.GameState.Playing)
                HandleLevelStarted(gameManager.LoadedLevelId ?? 0);
        }

        private void OnDisable()
        {
            StopPlaying();

            if (!gameManager)
                return;
        
            gameManager.OnLevelStarted -= HandleLevelStarted;
            gameManager.OnLevelFinishedFailed -= HandleLevelEnded;
            gameManager.OnLevelFinishedSuccess -= HandleLevelEnded;
        }

        private void OnValidate()
        {
            if (_maxPitch < _minPitch)
                (_minPitch, _maxPitch) = (_maxPitch, _minPitch);
            if (_maxInterval < _minInterval)
                (_minInterval, _maxInterval) = (_maxInterval, _minInterval);
        }

        private void HandleLevelStarted(int levelId) => 
            StartPlaying();

        private void HandleLevelEnded(int levelId) => 
            StopPlaying();

        private void StartPlaying()
        {
            if (isLevelActive)
                return;

            if (!_audioSource || _clips == null || _clips.Length == 0)
                return;

            isLevelActive = true;
            playRoutine = StartCoroutine(PlayRandomLoop());
        }

        private void StopPlaying()
        {
            isLevelActive = false;
        
            if (playRoutine == null) 
                return;
        
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        private IEnumerator PlayRandomLoop()
        {
            while (isLevelActive)
            {
                var waitTime = Random.Range(_minInterval, _maxInterval);
                yield return new WaitForSeconds(waitTime);

                if (!isLevelActive)
                    yield break;

                if (!_audioSource || _clips == null || _clips.Length == 0)
                    continue;

                var clip = _clips[Random.Range(0, _clips.Length)];
                if (!clip)
                    continue;

                _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
                _audioSource.PlayOneShot(clip);
            }
        }
    }
}
