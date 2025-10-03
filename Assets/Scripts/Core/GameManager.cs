using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.DevilMode;
using Music;
using Solo.MOST_IN_ONE;
using TMPro;
using UI.DevilificationProgress;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            Idle,
            Playing,
            GameOver
        }

        public static GameManager Instance;

        public static int Attempt;

        [Header("Timer Settings")] 
        [FormerlySerializedAs("startTime")] public float StartTime = 666f;
        [FormerlySerializedAs("timeBonus")] public int TimeBonus = 1;
        [FormerlySerializedAs("timePenalty")] public int TimePenalty = 10;
        [FormerlySerializedAs("showLabelDuration")] public float ShowLabelDuration = 1f;
        private float currentTime;

        [Header("UI")] 
        [FormerlySerializedAs("timerText")] public TextMeshProUGUI TimerText;
        [FormerlySerializedAs("timePenaltyLabel")] public GameObject TimePenaltyLabel;
        [FormerlySerializedAs("timeBonusLabel")] public GameObject TimeBonusLabel;
        [FormerlySerializedAs("gameOverPanel")] public GameObject GameOverPanel;
        [FormerlySerializedAs("gameOverDevilPanel")] public GameObject GameOverDevilPanel;
        [FormerlySerializedAs("levelPassedPanel")] public GameObject LevelPassedPanel;

        
        [Header("Levels")]
        [FormerlySerializedAs("levels")] [SerializeField] private List<LevelManager> _levels;

        private readonly Dictionary<int, int> timersByLevel = new()
        {
            {0, 30},
            {1, 60},
            {2, 90},
            {3, 666},
        };

        public event Action<int> OnLevelLoaded;
        public event Action<int> OnLevelStarted;
        public event Action<int> OnLevelFinishedSuccess;
        public event Action<int> OnLevelFinishedFailed;

        public GameState CurrentState { get; private set; }

        public int? LoadedLevelId => LoadedLevel?.LevelId;
        public LevelManager LoadedLevel { get; private set; }
        public bool InputEnabled { get; private set; } = true;

        private void OnEnable() => 
            Health.PlayerDied += FailLevel;

        private void OnDisable() => 
            Health.PlayerDied -= FailLevel;

        public void EnableInputs()
        {
            StartCoroutine(DelayedEnabling());

            IEnumerator DelayedEnabling()
            {
                yield return null;
                InputEnabled = true;
            }
        }

        public void DisableInputs() =>
            InputEnabled = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            CurrentState = GameState.Idle;

            if (TimerText != null)
                TimerText.gameObject.SetActive(false);

            LoadLevel(level: 0);
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing) 
                return;
            
            UpdateTimer();
            CheckWinCondition();
        }

        private void UpdateTimer()
        {
            if (DevilModeScenario.IsInDevilMode)
                return;

            currentTime -= Time.deltaTime;

            if (TimerText != null)
                TimerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";

            if (currentTime <= 0f)
                FailLevel();
        }

        private void CheckWinCondition()
        {
            if (LoadedLevel != null && LoadedLevel.SpawnManager.AllEnemiesDefeated())
                CompleteLevel();
        }

        public void LoadNextLeve()
        {
            LevelPassedPanel.SetActive(false);

            if (LoadedLevel == null)
            {
                LoadLevel(level: 0);
                return;
            }

            LoadLevel(LoadedLevel.LevelId + 1);
        }

        private void LoadLevel(int level)
        {
            if (LoadedLevelId == level)
            {
                Debug.LogError("Trying to load level again");
                return;
            }

            if (LoadedLevelId != null && LoadedLevelId != level)
                UnloadLevel(LoadedLevelId.Value);

            var levelManager = _levels.FirstOrDefault(x => x.LevelId == level);
            if (levelManager == null)
                return;

            levelManager.gameObject.SetActive(true);

            LoadedLevel = levelManager;

            if (LoadedLevel.LevelId != 0)
            {
                MusicManager.SceneLoaded?.Invoke(LoadedLevel.name);
                AmbientManager.SceneLoaded?.Invoke(LoadedLevel.name);
            }

            OnLevelLoaded?.Invoke(LoadedLevel.LevelId);
        }

        private void UnloadLevel(int level)
        {
            if (LoadedLevelId != level)
            {
                Debug.LogError("Trying to unload level that wasn't loaded");
                return;
            }

            var levelManager = _levels.FirstOrDefault(x => x.LevelId == level);
            if (levelManager == null)
                return;

            levelManager.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            if (LoadedLevel == null)
            {
                Debug.LogError("Trying to start the game while level isn't loaded");
                return;
            }

            currentTime = LoadedLevelId != null 
                          && timersByLevel.TryGetValue(LoadedLevelId.Value, out var time) 
                ? time 
                : StartTime;
            
            CurrentState = GameState.Playing;

            LoadedLevel.SpawnManager.SpawnWave();
            LoadedLevel.OnLevelStarted?.Invoke();

            if (TimerText != null)
                TimerText.gameObject.SetActive(true);

            OnLevelStarted?.Invoke(LoadedLevel.LevelId);

            if (LoadedLevel.name == "Level4")
                ActivateDevilLevelRules();
        }

        private void FailLevel()
        {
            if (!LoadedLevel)
            {
                Debug.LogError($"Trying to fail the level while {nameof(LoadedLevel)} is null");
                return;
            }

            Attempt++;
            CurrentState = GameState.GameOver;

            if (TimerText)
                TimerText.gameObject.SetActive(false);

            LoadedLevel.SpawnManager.DestroyAll();

            var overPanel = DevilModeScenario.IsInDevilMode ? GameOverDevilPanel : GameOverPanel;

            overPanel.SetActive(true);

            OnLevelFinishedFailed?.Invoke(LoadedLevel.LevelId);
            MusicManager.StopActiveMusic?.Invoke(() =>
            {
                JingleManager.PlayLoseJingle?.Invoke();
                MusicManager.SceneLoaded?.Invoke(MusicManager.MenuBetweenLevelsSceneName);
            });
            
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Failure);
            
            DevilificationProgress.ResetProgress();

            Debug.Log("Game Over!");
        }

        private void CompleteLevel()
        {
            if (LoadedLevel == null)
            {
                Debug.LogError($"Trying to complete the level while {nameof(LoadedLevel)} is null");
                return;
            }

            CurrentState = GameState.Idle;

            if (TimerText != null)
                TimerText.gameObject.SetActive(false);

            LoadedLevel.SpawnManager.DestroyAll();

            LevelPassedPanel.SetActive(true);

            MusicManager.StopActiveMusic?.Invoke(() =>
            {
                JingleManager.PlayWinJingle?.Invoke();
                MusicManager.SceneLoaded?.Invoke(MusicManager.MenuBetweenLevelsSceneName);
            });

            OnLevelFinishedSuccess?.Invoke(LoadedLevel.LevelId);

            Debug.Log("Level Passed!");
        }

        public void RestartGame()
        {
            var overPanel = DevilModeScenario.IsInDevilMode ? GameOverDevilPanel : GameOverPanel;
            overPanel.SetActive(false);
            
            DevilificationProgress.ResetProgress();

            var sceneName = SceneManager.GetActiveScene().name;
            MusicManager.SceneLoaded?.Invoke(sceneName);
            AmbientManager.SceneLoaded?.Invoke(sceneName);
            SceneManager.LoadScene(sceneName);
        }

        public void AddTime(int bonus)
        {
            currentTime += bonus;

            StartCoroutine(ShowTimeLabel(TimeBonusLabel, bonus));
        }

        public void SubtractTime()
        {
            currentTime -= TimePenalty;
            if (currentTime < 0)
                currentTime = 0;

            StartCoroutine(ShowTimeLabel(TimePenaltyLabel, -TimePenalty));
        }

        private IEnumerator ShowTimeLabel(GameObject label, int amount)
        {
            if (label == null)
                yield break;

            if (amount < 0)
                label.GetComponentInChildren<TextMeshProUGUI>().text = $"{amount}";
            else if (amount > 0)
                label.GetComponentInChildren<TextMeshProUGUI>().text = $"+{amount}";

            label.SetActive(true);
            yield return new WaitForSeconds(ShowLabelDuration);
            label.SetActive(false);
        }

        private void ActivateDevilLevelRules()
        {
            DevilModeScenario.ForceDevilMode?.Invoke();
            TimerText.gameObject.SetActive(false);
        }
    }
}
