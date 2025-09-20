using Core;
using UnityEngine;

namespace Music
{
    public class MusicCallback : MonoBehaviour
    {
        public void PlayMusic(string levelName)
        {
            MusicManager.SceneLoaded?.Invoke(levelName);
            AmbientManager.SceneLoaded?.Invoke(levelName);
        }
    }
}
