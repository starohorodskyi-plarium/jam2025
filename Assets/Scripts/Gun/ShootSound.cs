using UnityEngine;

public class ShootSound : MonoBehaviour
{
    [System.Serializable]
    private struct WeightedClip
    {
        public AudioClip clip;
        public float weight;
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private WeightedClip[] clips;
    [SerializeField] private float destroyAfterSeconds = 2f;

    private void Start()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        var chosen = SelectClipByWeight(clips);
        if (chosen && audioSource)
        {
            audioSource.PlayOneShot(chosen);
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

    private static AudioClip SelectClipByWeight(WeightedClip[] weightedClips)
    {
        if (weightedClips == null || weightedClips.Length == 0) return null;

        float totalWeight = 0f;
        for (int i = 0; i < weightedClips.Length; i++)
        {
            if (weightedClips[i].clip == null || weightedClips[i].weight <= 0f) continue;
            totalWeight += weightedClips[i].weight;
        }

        if (totalWeight <= 0f)
        {
            for (int i = 0; i < weightedClips.Length; i++)
            {
                if (weightedClips[i].clip != null) return weightedClips[i].clip;
            }
            return null;
        }

        float randomPoint = Random.value * totalWeight;
        float cumulative = 0f;

        for (int i = 0; i < weightedClips.Length; i++)
        {
            if (weightedClips[i].clip == null || weightedClips[i].weight <= 0f) continue;
            cumulative += weightedClips[i].weight;
            if (randomPoint <= cumulative)
            {
                return weightedClips[i].clip;
            }
        }

        return null;
    }
}
