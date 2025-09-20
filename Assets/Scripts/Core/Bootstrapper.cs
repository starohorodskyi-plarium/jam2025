using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Core
{
    public class Bootstrapper : MonoBehaviour
    {
        [FormerlySerializedAs("startMenuSceneName")] [SerializeField] private string _startMenuSceneName;
        [FormerlySerializedAs("sceneLoader")] [SerializeField] private UnityEvent<string> _sceneLoader;
   
        private void Awake() => 
            _sceneLoader?.Invoke(_startMenuSceneName);
    }
}
