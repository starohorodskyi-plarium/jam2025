using UnityEngine;

public class ShootSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    [SerializeField] private float pitchMin = 1f;
    [SerializeField] private float pitchMax = 1f;
    [SerializeField] private float destroyAfterSeconds = 2f;

    private void Start()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource)
        {
            float min = Mathf.Min(pitchMin, pitchMax);
            float max = Mathf.Max(pitchMin, pitchMax);
            audioSource.pitch = Random.Range(min, max);

            if (clip)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        if (destroyAfterSeconds > 0f)
        {
            Destroy(gameObject, destroyAfterSeconds);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
