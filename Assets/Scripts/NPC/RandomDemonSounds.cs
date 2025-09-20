using System.Collections;
using Core;
using UnityEngine;

namespace NPC
{
    public class RandomDemonSounds : MonoBehaviour
    {
        [Header("Audio")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;

        [Header("Pitch Settings")] 
        [SerializeField] [Range(0.1f, 3f)] private float minPitch = 0.9f;
        [SerializeField] [Range(0.1f, 3f)] private float maxPitch = 1.1f;

        [Header("Interval (seconds)")]
        [SerializeField] [Min(0f)] private float minInterval = 6f;
        [SerializeField] [Min(0f)] private float maxInterval = 12f;

        private Coroutine playRoutine;
        private GameManager gameManager;
        private bool isLevelActive;

        private void Awake()
        {
            if (!audioSource)
                audioSource = GetComponent<AudioSource>();
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
            if (maxPitch < minPitch)
                (minPitch, maxPitch) = (maxPitch, minPitch);
            if (maxInterval < minInterval)
                (minInterval, maxInterval) = (maxInterval, minInterval);
        }

        private void HandleLevelStarted(int levelId)
        {
            StartPlaying();
        }

        private void HandleLevelEnded(int levelId)
        {
            StopPlaying();
        }

        private void StartPlaying()
        {
            if (isLevelActive)
                return;

            if (!audioSource || clips == null || clips.Length == 0)
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
                var waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                if (!isLevelActive)
                    yield break;

                if (!audioSource || clips == null || clips.Length == 0)
                    continue;

                var clip = clips[Random.Range(0, clips.Length)];
                if (!clip)
                    continue;

                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
