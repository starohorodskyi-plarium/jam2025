using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class NPCShotAudioReaction : MonoBehaviour
    {
        [Header("Audio")] 
        [FormerlySerializedAs("audioSource")][SerializeField] private AudioSource _audioSource;
        
        [Header("Death Sounds - Humans")]
        [FormerlySerializedAs("humanMaleDeathClips")]  [SerializeField] private AudioClip[] _humanMaleDeathClips;
        [FormerlySerializedAs("humanFemaleDeathClips")] [SerializeField] private AudioClip[] _humanFemaleDeathClips;
        
        [Header("Death Sounds - Demons")]
        [FormerlySerializedAs("demonMaleDeathClips")] [SerializeField] private AudioClip[] _demonMaleDeathClips;
        [FormerlySerializedAs("demonFemaleDeathClips")] [SerializeField] private AudioClip[] _demonFemaleDeathClips;

        private AudioClip GetRandomHumanDeathClip(NPCController.Gender gender) =>
            GetRandomClip(gender == NPCController.Gender.Male ? _humanMaleDeathClips : _humanFemaleDeathClips);

        private AudioClip GetRandomDemonDeathClip(NPCController.Gender gender) =>
            GetRandomClip(gender == NPCController.Gender.Male ? _demonMaleDeathClips : _demonFemaleDeathClips);

        public void PlayHumanDeath() =>
            PlayHumanDeath(NPCController.Gender.Male);

        public void PlayDemonDeath() =>
            PlayDemonDeath(NPCController.Gender.Male);

        public void PlayHumanDeath(NPCController.Gender gender) =>
            PlayClip(GetRandomHumanDeathClip(gender));

        public void PlayDemonDeath(NPCController.Gender gender) =>
            PlayClip(GetRandomDemonDeathClip(gender));

        private AudioClip GetRandomClip(AudioClip[] clips) =>
            (clips == null || clips.Length == 0) ? null : clips[Random.Range(0, clips.Length)];

        private void PlayClip(AudioClip clip)
        {
            if (clip == null || _audioSource == null)
                return;
            
            _audioSource.PlayOneShot(clip);
            Destroy(gameObject, 4f);
        }
    }
}
