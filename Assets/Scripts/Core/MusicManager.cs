using System;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private string initialSceneName = "MainMenu";
        [SerializeField] private StringAudioClipDictionary musicMap;
        
        [SerializeField] private AudioSource audioSource;
        
        [SerializeField] [Range(0f, 5f)]private float transitionDuration = 3f;

        public static Action<string> SceneLoaded;

        private void Start() =>
            PlayNewMusic(initialSceneName);
        
        private void OnEnable() =>
            SceneLoaded += OnSceneLoaded;

        private void OnDisable() => 
            SceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(string sceneName)
        {
            if(audioSource.isPlaying)
                audioSource.DOFade(0f, transitionDuration).OnComplete(() => PlayNewMusic(sceneName));
            else 
                PlayNewMusic(sceneName);
        }

        private void PlayNewMusic(string sceneName)
        {
            if (!musicMap.TryGetValue(sceneName, out var value)) 
                return;
            
            if(!value || !audioSource)
                return;
            
            audioSource.clip = value;
            audioSource.Play();
            audioSource.DOFade(1f, transitionDuration);
        }
    }
    
    [Serializable]
    public class StringAudioClipDictionary : SerializableDictionary<string, AudioClip> 
    {
    }
}
