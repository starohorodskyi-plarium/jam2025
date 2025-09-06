using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class GameStateListener : MonoBehaviour
    {
        [SerializeField] private int _level;
        [SerializeField] private bool _ignoreLevel;
        
        public UnityEvent OnLevelStarted;
        public UnityEvent OnLevelFinished;

        private void OnEnable()
        {
            FindFirstObjectByType<GameManager>().OnLevelStarted += LevelStarted;
            FindFirstObjectByType<GameManager>().OnLevelFinishedSuccess += LevelFinishedSuccess;
        }

        private void OnDisable()
        {
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
                return;
            
            gameManager.OnLevelStarted -= LevelStarted;
            gameManager.OnLevelFinishedSuccess -= LevelFinishedSuccess;
        }

        private void LevelFinishedSuccess(int level)
        {
            if (_ignoreLevel || level == _level)
                OnLevelFinished?.Invoke();
        }

        private void LevelStarted(int level)
        {
            if (_ignoreLevel || level == _level)
                OnLevelStarted?.Invoke();
        }
    }
}
