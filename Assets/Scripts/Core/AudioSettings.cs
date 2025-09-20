using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Core
{
    public class AudioSettings : MonoBehaviour
    {
        private const string SoundsSettingKey = "Sounds";
        private const string MusicSettingKey = "Music";
        
        [FormerlySerializedAs("mixer")] [SerializeField] private AudioMixer _mixer;
  
        [Space(20)]
        [FormerlySerializedAs("soundKey")] [SerializeField] private string _soundKey;
        [FormerlySerializedAs("musicKey")] [SerializeField] private string _musicKey;

        private const float MutedVolumeValue = -80f;

        private void Start()
        {
            if (PlayerPrefs.HasKey(SoundsSettingKey))
                _mixer.SetFloat(_soundKey, PlayerPrefs.GetFloat(SoundsSettingKey)); 
            
            if (PlayerPrefs.HasKey(MusicSettingKey))
                _mixer.SetFloat(_musicKey, PlayerPrefs.GetFloat(MusicSettingKey));
        }

        private bool IsSoundsMuted()
        {
            _mixer.GetFloat(_soundKey, out var volume);
            return Mathf.Approximately(volume, MutedVolumeValue);
        }

        private bool IsMusicMuted()
        {
            _mixer.GetFloat(_musicKey, out var volume);
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
            _mixer.SetFloat(_soundKey, -80f);
        }

        private void UnmuteSounds()
        {
            PlayerPrefs.SetFloat(SoundsSettingKey, 0f);
            _mixer.SetFloat(_soundKey, 0f);
        }

        private void MuteMusic()
        {
            PlayerPrefs.SetFloat(MusicSettingKey, -80f);
            _mixer.SetFloat(_musicKey, -80f);
        }

        private void UnmuteMusic()
        {
            PlayerPrefs.SetFloat(MusicSettingKey, 0f);
            _mixer.SetFloat(_musicKey, 0f);
        }
    }
}
