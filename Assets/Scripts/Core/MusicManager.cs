using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class MusicManager : MonoBehaviour
    {
        public const string MenuBetweenLevelsSceneName = "MenuBetweenLevels";

        [FormerlySerializedAs("initialSceneName")] [SerializeField] private string _initialSceneName = "MainMenu";
        [FormerlySerializedAs("musicMap")] [SerializeField] private StringAudioClipDictionary _musicMap;
        
        [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource _audioSource;
        [FormerlySerializedAs("maxVolume")] [SerializeField] [Range	(0f,1f)] private float _maxVolume = 0.1f;
        
        [FormerlySerializedAs("transitionDuration")] [SerializeField] [Range(0f, 5f)]private float _transitionDuration = 3f;

        public static Action<string> SceneLoaded;
        public static Action<Action> StopActiveMusic;

        private void Start() =>
            PlayNewMusic(_initialSceneName);

        private void OnEnable()
        {
            StopActiveMusic += StopMusic; 
            SceneLoaded += OnSceneLoaded;  
        }

        private void OnDisable()
        {
            StopActiveMusic -= StopMusic; 
            SceneLoaded -= OnSceneLoaded;
        }
            

        private void OnSceneLoaded(string sceneName)
        {
            if(_audioSource.isPlaying)
                _audioSource.DOFade(0f, _transitionDuration).OnComplete(() => PlayNewMusic(sceneName));
            else 
                PlayNewMusic(sceneName);
        }

        private void StopMusic(Action actionAfterStop)
        {
            if (!_audioSource.isPlaying) 
                return;
            
            _audioSource.DOKill();
            _audioSource.DOFade(0f, 0.4f).OnComplete	(() => actionAfterStop?.Invoke());
        }

        private void PlayNewMusic(string sceneName)
        {
            if (!_musicMap.TryGetValue(sceneName, out var value)) 
                return;
            
            if(!value || !_audioSource)
                return;
            
            _audioSource.clip = value;
            _audioSource.Play();
            _audioSource.DOFade(_maxVolume, _transitionDuration);
        }
    }
    
    [Serializable]
    public class StringAudioClipDictionary : SerializableDictionary<string, AudioClip> 
    {
    }
}
