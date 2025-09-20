using System;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class AmbientManager : MonoBehaviour
    {
        [SerializeField] private string initialSceneName = "MainMenu";
        [SerializeField] private StringAudioClipDictionary ambientMap;
        [SerializeField] private AudioSource audioSource;
        [SerializeField][Range(0f, 1f)] private float maxVolume = 0.1f;
        [SerializeField][Range(0f, 5f)] private float transitionDuration = 3f;

        public static Action<string> SceneLoaded;
        public static Action<Action> StopActiveAmbient;

        private void Start() => PlayNewAmbient(initialSceneName);
        private void OnEnable() { StopActiveAmbient += StopAmbient; SceneLoaded += OnSceneLoaded; }
        private void OnDisable() { StopActiveAmbient -= StopAmbient; SceneLoaded -= OnSceneLoaded; }

        private void OnSceneLoaded(string sceneName)
        {
            if (audioSource.isPlaying)
                audioSource.DOFade(0f, transitionDuration).OnComplete(() => PlayNewAmbient(sceneName));
            else
                PlayNewAmbient(sceneName);
        }

        private void StopAmbient(Action actionAfterStop)
        {
            if (!audioSource.isPlaying) return;
            audioSource.DOKill();
            audioSource.DOFade(0f, 0.4f).OnComplete(() => actionAfterStop?.Invoke());
        }

        private void PlayNewAmbient(string sceneName)
        {
            if (!ambientMap.TryGetValue(sceneName, out var value)) return;
            if (!value || !audioSource) return;
            audioSource.clip = value;
            audioSource.Play();
            audioSource.DOFade(maxVolume, transitionDuration);
        }
    }
}
