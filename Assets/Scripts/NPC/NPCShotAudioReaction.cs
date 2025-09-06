using UnityEngine;

public class NPCShotAudioReaction : MonoBehaviour
{
    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    [Header("Death Sounds - Humans")]
    [SerializeField] private AudioClip[] humanMaleDeathClips;
    [SerializeField] private AudioClip[] humanFemaleDeathClips;

    [Header("Death Sounds - Demons")]
    [SerializeField] private AudioClip[] demonMaleDeathClips;
    [SerializeField] private AudioClip[] demonFemaleDeathClips;

    public AudioClip GetRandomHumanDeathClip(NPCController.Gender gender) =>
        GetRandomClip(gender == NPCController.Gender.Male ? humanMaleDeathClips : humanFemaleDeathClips);

    public AudioClip GetRandomDemonDeathClip(NPCController.Gender gender) =>
        GetRandomClip(gender == NPCController.Gender.Male ? demonMaleDeathClips : demonFemaleDeathClips);

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
        if (clip == null || audioSource == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }
}
