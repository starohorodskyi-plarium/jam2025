using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float startTime = 60f;
    public float timeBonus = 1f;
    public float timePenalty = 5f;
    public float showLabelDuration = 1f;
    private float currentTime;
    
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject timePenaltyLabel;
    public GameObject timeBonusLabel;
    public GameObject gameOverPanel;
    public GameObject levelPassedPanel;

    [Header("Levels")] 
    [SerializeField] private List<GameObject> levels;

    public event Action<int> OnLevelStarted;
    public event Action<int> OnLevelFinishedSuccess;
    public event Action<int> OnLevelFinishedFailed;
    
    public GameState CurrentState { get; private set; }
    
    public int? LoadedLevel { get; private set; }
    public bool InputEnabled { get; private set; } = true;

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
        currentTime -= Time.deltaTime;
        
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        
        if (currentTime <= 0f)
            FailLevel();
    }
    
    private void CheckWinCondition()
    {
        if (SpawnManager.Instance.AllEnemiesDefeated())
            CompleteLevel();
    }

    public void LoadLevel(int level)
    {
        if (LoadedLevel == level)
        {
            Debug.LogError("Trying to load level again");
            return;
        }

        if (LoadedLevel != null && LoadedLevel != level) 
            UnloadLevel(LoadedLevel.Value);
        
        var levelGO = levels.ElementAtOrDefault(level);
        if (levelGO == null)
            return;
        
        levelGO.SetActive(true);
        LoadedLevel = level;
    }

    public void UnloadLevel(int level)
    {
        if (LoadedLevel != level)
        {
            Debug.LogError("Trying to unload level that wasn't loaded");
            return;
        }
        
        var levelGO = levels.ElementAtOrDefault(level);
        if (levelGO == null)
            return;
        
        levelGO.SetActive(false);
    }
    
    public void StartGame()
    {
        currentTime = startTime;
        CurrentState = GameState.Playing;

        SpawnManager.Instance.SpawnWave();
        
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        
        OnLevelStarted?.Invoke(1);
    }

    public void FailLevel()
    {
        CurrentState = GameState.GameOver;

        if (timerText != null)
            timerText.gameObject.SetActive(false);
        
        SpawnManager.Instance.DestroyAll();
        
        gameOverPanel.SetActive(true);
        
        OnLevelFinishedSuccess?.Invoke(1);
        
        Debug.Log("Game Over!");
    }

    public void CompleteLevel()
    {
        CurrentState = GameState.Idle;
        
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        
        SpawnManager.Instance.DestroyAll();
        
        levelPassedPanel.SetActive(true);
        
        OnLevelFinishedSuccess?.Invoke(1);
        
        Debug.Log("Level Passed!");
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void OpenNextLevel()
    {
        levelPassedPanel.SetActive(false);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddTime()
    {
        currentTime += timeBonus;
        
        StartCoroutine(ShowTimeLabel(timeBonusLabel));
    }

    public void SubtractTime()
    {
        currentTime -= timePenalty;
        if (currentTime < 0) currentTime = 0;

        StartCoroutine(ShowTimeLabel(timePenaltyLabel));
    }
    
    private IEnumerator ShowTimeLabel(GameObject label)
    {
        if (label == null) 
            yield break;
        
        label.SetActive(true);
        yield return new WaitForSeconds(showLabelDuration);
        label.SetActive(false);
    }
}
