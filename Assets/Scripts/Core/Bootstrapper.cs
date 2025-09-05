using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private string startMenuSceneName;
        [SerializeField] private UnityEvent<string> sceneLoader;
   
        private void Awake() => 
            sceneLoader?.Invoke(startMenuSceneName);
    }
}
