using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class ClickSound : MonoBehaviour
    {
        [FormerlySerializedAs("audioPrefab")] [SerializeField] private GameObject _audioPrefab;
        [FormerlySerializedAs("clipSound")] [SerializeField] private AudioClip _clipSound;
        
        [FormerlySerializedAs("volume")] [SerializeField] [Range(0f,1f)] private float _volume = 1f;

        public void Click()
        {
            if (!_clipSound || !_audioPrefab) 
                return;
            
            var click = Instantiate(_audioPrefab);
            click.GetComponent<AudioSource>().volume = _volume;
            click.GetComponent<AudioSource>().PlayOneShot(_clipSound);
            
            Destroy(click, 3f);
        }
    }
}
