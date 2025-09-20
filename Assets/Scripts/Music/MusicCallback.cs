using Core;
using UnityEngine;

public class MusicCallback : MonoBehaviour
{
    public void PlayMusic(string levelName)
    {
        MusicManager.SceneLoaded?.Invoke(levelName);
        AmbientManager.SceneLoaded?.Invoke(levelName);
    }
}
