using System;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class MusicManager : MonoBehaviour
    {
        public static string MenuBetweenLevelsSceneName = "MenuBetweenLevels";
        
        [SerializeField] private string initialSceneName = "MainMenu";
        [SerializeField] private StringAudioClipDictionary musicMap;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] [Range	(0f,1f)] private float maxVolume = 0.1f;
        
        [SerializeField] [Range(0f, 5f)]private float transitionDuration = 3f;

        public static Action<string> SceneLoaded;
        public static Action<Action> StopActiveMusic;

        private void Start() =>
            PlayNewMusic(initialSceneName);

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
            if(audioSource.isPlaying)
                audioSource.DOFade(0f, transitionDuration).OnComplete(() => PlayNewMusic(sceneName));
            else 
                PlayNewMusic(sceneName);
        }

        private void StopMusic(Action actionAfterStop)
        {
            if (!audioSource.isPlaying) 
                return;
            
            audioSource.DOKill();
            audioSource.DOFade(0f, 0.4f).OnComplete	(() => actionAfterStop?.Invoke());
        }

        private void PlayNewMusic(string sceneName)
        {
            if (!musicMap.TryGetValue(sceneName, out var value)) 
                return;
            
            if(!value || !audioSource)
                return;
            
            audioSource.clip = value;
            audioSource.Play();
            audioSource.DOFade(maxVolume, transitionDuration);
        }
    }
    
    [Serializable]
    public class StringAudioClipDictionary : SerializableDictionary<string, AudioClip> 
    {
    }
}
