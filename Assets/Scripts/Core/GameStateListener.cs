using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class GameStateListener : MonoBehaviour
    {
        [SerializeField] private int _level;
        [SerializeField] private bool _ignoreLevel;

        public UnityEvent OnLevelLoaded;
        public UnityEvent OnLevelStarted;
        public UnityEvent OnLevelFinishedFailed;
        public UnityEvent OnLevelFinishedSuccess;

        private void OnEnable()
        {
            FindFirstObjectByType<GameManager>().OnLevelLoaded += LevelLoaded;
            FindFirstObjectByType<GameManager>().OnLevelStarted += LevelStarted;
            FindFirstObjectByType<GameManager>().OnLevelFinishedFailed += LevelFinishedFailed;
            FindFirstObjectByType<GameManager>().OnLevelFinishedSuccess += LevelFinishedSuccess;
        }

        private void OnDisable()
        {
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
                return;

            gameManager.OnLevelLoaded -= LevelLoaded;
            gameManager.OnLevelStarted -= LevelStarted;
            gameManager.OnLevelFinishedFailed -= LevelFinishedFailed;
            gameManager.OnLevelFinishedSuccess -= LevelFinishedSuccess;
        }
        
        private void LevelLoaded(int level)
        {
            if (_ignoreLevel || level == _level)
                OnLevelLoaded?.Invoke();
        }
        
        private void LevelFinishedFailed(int level)
        {
            if (_ignoreLevel || level == _level)
                OnLevelFinishedFailed?.Invoke();
        }

        private void LevelFinishedSuccess(int level)
        {
            if (_ignoreLevel || level == _level)
                OnLevelFinishedSuccess?.Invoke();
        }

        private void LevelStarted(int level)
        {
            if (_ignoreLevel || level == _level)
                OnLevelStarted?.Invoke();
        }
    }
}
