using UnityEngine;
using UnityEngine.Serialization;

namespace Gun
{
    public class ShootSound : MonoBehaviour
    {
        [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource _audioSource;
        [FormerlySerializedAs("clip")] [SerializeField] private AudioClip _clip;
        
        [FormerlySerializedAs("pitchMin")] [SerializeField] private float _pitchMin = 1f;
        [FormerlySerializedAs("pitchMax")] [SerializeField] private float _pitchMax = 1f;
        [FormerlySerializedAs("destroyAfterSeconds")] [SerializeField] private float _destroyAfterSeconds = 2f;

        private void Start()
        {
            if (!_audioSource) 
                _audioSource = GetComponent<AudioSource>();

            if (_audioSource)
            {
                var min = Mathf.Min(_pitchMin, _pitchMax);
                var max = Mathf.Max(_pitchMin, _pitchMax);
                _audioSource.pitch = Random.Range(min, max);

                if (_clip) 
                    _audioSource.PlayOneShot(_clip);
            }

            if (_destroyAfterSeconds > 0f)
                Destroy(gameObject, _destroyAfterSeconds);
            else
                Destroy(gameObject);
        }
    }
}
