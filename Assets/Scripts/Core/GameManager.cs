using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Gameplay.DevilMode;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Idle, 
        Playing, 
        GameOver
    }
    
    public static GameManager Instance; 

    [Header("Timer Settings")]
    public float startTime = 666f;
    public int timeBonus = 1;
    public int timePenalty = 10;
    public float showLabelDuration = 1f;
    private float currentTime;
    
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject timePenaltyLabel;
    public GameObject timeBonusLabel;
    public GameObject gameOverPanel;
    public GameObject gameOverDevilPanel;
    public GameObject levelPassedPanel;

    [Header("Levels")] 
    [SerializeField] private List<LevelManager> levels;

    public Dictionary<int, int> timersByLevel = new Dictionary<int, int>()
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

    private void OnEnable()
    {
        Health.PlayerDied += FailLevel;
    }

    private void OnDisable()
    {
        Health.PlayerDied -= FailLevel;
    }

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
        
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        
        LoadLevel(level: 0);
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            UpdateTimer();
            CheckWinCondition();
        }
    }

    private void UpdateTimer()
    {
        if (DevilModeScenario.IsInDevilMode)
            return;
        
        currentTime -= Time.deltaTime;
        
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        
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
        levelPassedPanel.SetActive(false);
        
        if (LoadedLevel == null)
        {
            LoadLevel(level: 0);
            return;
        }
        
        LoadLevel(LoadedLevel.LevelId + 1);
    }

    public void LoadLevel(int level)
    {
        if (LoadedLevelId == level)
        {
            Debug.LogError("Trying to load level again");
            return;
        }

        if (LoadedLevelId != null && LoadedLevelId != level) 
            UnloadLevel(LoadedLevelId.Value);
        
        var levelManager = levels.FirstOrDefault(x => x.LevelId == level);
        if (levelManager == null)
            return;
        
        levelManager.gameObject.SetActive(true);

        LoadedLevel = levelManager;
        
        if(LoadedLevel.LevelId	!= 0) 
            MusicManager.SceneLoaded?.Invoke(LoadedLevel.name);
        
        OnLevelLoaded?.Invoke(LoadedLevel.LevelId);
    }

    public void UnloadLevel(int level)
    {
        if (LoadedLevelId != level)
        {
            Debug.LogError("Trying to unload level that wasn't loaded");
            return;
        }
        
        var levelManager = levels.FirstOrDefault(x => x.LevelId == level);
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
        
        currentTime = timersByLevel.TryGetValue(LoadedLevelId.Value, out var time) ? time : startTime;
        CurrentState = GameState.Playing;

        LoadedLevel.SpawnManager.SpawnWave();
        
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        
        OnLevelStarted?.Invoke(LoadedLevel.LevelId);

        if (LoadedLevel.name == "Level4")
        {
            ActivateDevilLevelRules();
        }
    }

    public void FailLevel()
    {
        if (LoadedLevel == null)
        {
            Debug.LogError($"Trying to fail the level while {nameof(LoadedLevel)} is null");
            return;
        }
        
        CurrentState = GameState.GameOver;

        if (timerText != null)
            timerText.gameObject.SetActive(false);
        
        LoadedLevel.SpawnManager.DestroyAll();
        
        var overPanel = DevilModeScenario.IsInDevilMode ? gameOverDevilPanel : gameOverPanel;
        
        overPanel.SetActive(true);
        
        OnLevelFinishedFailed?.Invoke(LoadedLevel.LevelId);
        MusicManager.StopActiveMusic?.Invoke(() => JingleManager.PlayLoseJingle?.Invoke());
        
        Debug.Log("Game Over!");
    }

    public void CompleteLevel()
    {
        if (LoadedLevel == null)
        {
            Debug.LogError($"Trying to complete the level while {nameof(LoadedLevel)} is null");
            return;
        }
        
        CurrentState = GameState.Idle;
        
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        
        LoadedLevel.SpawnManager.DestroyAll();
        
        levelPassedPanel.SetActive(true);
        
        MusicManager.StopActiveMusic?.Invoke(() => JingleManager.PlayWinJingle?.Invoke());
        
        DevilificationProgress.OnSetSmooth?.Invoke((LoadedLevel.LevelId + 1)/DevilificationProgress.DevilificationLevelId);
        
        OnLevelFinishedSuccess?.Invoke(LoadedLevel.LevelId);
        
        Debug.Log("Level Passed!");
    }

    public void RestartGame()
    {
        var overPanel = DevilModeScenario.IsInDevilMode ? gameOverDevilPanel : gameOverPanel;
        overPanel.SetActive(false);
        
        var sceneName = SceneManager.GetActiveScene().name;
        MusicManager.SceneLoaded?.Invoke(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void AddTime(int bonus)
    {
        currentTime += bonus;
        
        StartCoroutine(ShowTimeLabel(timeBonusLabel, bonus));
    }

    public void SubtractTime()
    {
        currentTime -= timePenalty;
        if (currentTime < 0) 
            currentTime = 0;

        StartCoroutine(ShowTimeLabel(timePenaltyLabel, -timePenalty));
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
        yield return new WaitForSeconds(showLabelDuration);
        label.SetActive(false);
    }

    private void ActivateDevilLevelRules()
    {
        DevilModeScenario.ForceDevilMode?.Invoke();
        timerText.gameObject.SetActive(false);
    }
}
