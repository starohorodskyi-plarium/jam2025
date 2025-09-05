using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    public class AudioSettings : MonoBehaviour
    {
        private const string SoundsSettingKey = "Sounds";
        private const string MusicSettingKey = "Music";
        
        [SerializeField] private AudioMixer mixer;
        [Space(20)]
        [SerializeField] private string soundKey;
        [SerializeField] private string musicKey;

        private const float MutedVolumeValue = -80f;

        private void Start()
        {
            if (PlayerPrefs.HasKey(SoundsSettingKey))
                mixer.SetFloat(soundKey, PlayerPrefs.GetFloat(SoundsSettingKey)); 
            
            if (PlayerPrefs.HasKey(MusicSettingKey))
                mixer.SetFloat(musicKey, PlayerPrefs.GetFloat(MusicSettingKey));
        }

        public bool IsSoundsMuted()
        {
            mixer.GetFloat(soundKey, out var volume);
            return Mathf.Approximately(volume, MutedVolumeValue);
        }

        public bool IsMusicMuted()
        {
            mixer.GetFloat(musicKey, out var volume);
            return Mathf.Approximately(volume, MutedVolumeValue);
        }
        
        public void SwitchSounds()
        {
            if (IsSoundsMuted())
                UnmuteSounds();
            else
                MuteSounds();
        }
        
        public void SwitchMusic()
        {
            if (IsMusicMuted())
                UnmuteMusic();
            else
                MuteMusic();
        }

        private void MuteSounds()
        {
            PlayerPrefs.SetFloat(SoundsSettingKey, -80f);
            mixer.SetFloat(soundKey, -80f);
        }

        private void UnmuteSounds()
        {
            PlayerPrefs.SetFloat(SoundsSettingKey, 0f);
            mixer.SetFloat(soundKey, 0f);
        }

        private void MuteMusic()
        {
            PlayerPrefs.SetFloat(MusicSettingKey, -80f);
            mixer.SetFloat(musicKey, -80f);
        }

        private void UnmuteMusic()
        {
            PlayerPrefs.SetFloat(MusicSettingKey, 0f);
            mixer.SetFloat(musicKey, 0f);
        }
    }
}
