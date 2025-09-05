using TMPro;
using UnityEngine;

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
    private float timeBonus = 1f;
    private float timePenalty = 5f;
    private float currentTime;
    
    [Header("UI")]
    public TextMeshProUGUI timerText;
    
    public GameState CurrentState { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        CurrentState = GameState.Idle;   // default state before clicking start
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (CurrentState == GameState.Playing)
            UpdateTimer();
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        
        if (currentTime <= 0f)
            EndGame();
    }
    
    public void StartGame()
    {
        currentTime = startTime;
        CurrentState = GameState.Playing;

        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        CurrentState = GameState.GameOver;

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        Debug.Log("Game Over!");
    }

    public void AddTime()
    {
        currentTime += timeBonus;
    }

    public void SubtractTime()
    {
        currentTime -= timePenalty;
        if (currentTime < 0) currentTime = 0;
    }
}
