using UnityEngine;

namespace Core
{
    public class ClickSound : MonoBehaviour
    {
        [SerializeField] private GameObject audioPrefab;
        [SerializeField] private AudioClip clipSound;
        
        [SerializeField] [Range(0f,1f)] private float volume = 1f;

        public void Click()
        {
            if (!clipSound || !audioPrefab) 
                return;
            
            var click = Instantiate(audioPrefab);
            click.GetComponent<AudioSource>().volume = volume;
            click.GetComponent<AudioSource>().PlayOneShot(clipSound);
            
            Destroy(click, 3f);
        }
    }
}
