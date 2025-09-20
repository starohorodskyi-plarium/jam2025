using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class AmbientManager : MonoBehaviour
    {
        [FormerlySerializedAs("initialSceneName")] [SerializeField] private string _initialSceneName = "MainMenu";
        [FormerlySerializedAs("ambientMap")] [SerializeField] private StringAudioClipDictionary _ambientMap;
        [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource _audioSource;
        [FormerlySerializedAs("maxVolume")] [SerializeField][Range(0f, 1f)] private float _maxVolume = 0.1f;
        [FormerlySerializedAs("transitionDuration")] [SerializeField][Range(0f, 5f)] private float _transitionDuration = 3f;

        public static Action<string> SceneLoaded;
        public static Action<Action> StopActiveAmbient;

        private void Start() => PlayNewAmbient(_initialSceneName);
        private void OnEnable() { StopActiveAmbient += StopAmbient; SceneLoaded += OnSceneLoaded; }
        private void OnDisable() { StopActiveAmbient -= StopAmbient; SceneLoaded -= OnSceneLoaded; }

        private void OnSceneLoaded(string sceneName)
        {
            if (_audioSource.isPlaying)
                _audioSource.DOFade(0f, _transitionDuration).OnComplete(() => PlayNewAmbient(sceneName));
            else
                PlayNewAmbient(sceneName);
        }

        private void StopAmbient(Action actionAfterStop)
        {
            if (!_audioSource.isPlaying) return;
            _audioSource.DOKill();
            _audioSource.DOFade(0f, 0.4f).OnComplete(() => actionAfterStop?.Invoke());
        }

        private void PlayNewAmbient(string sceneName)
        {
            if (!_ambientMap.TryGetValue(sceneName, out var value)) return;
            if (!value || !_audioSource) return;
            _audioSource.clip = value;
            _audioSource.Play();
            _audioSource.DOFade(_maxVolume, _transitionDuration);
        }
    }
}
