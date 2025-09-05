using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing, 
        GameOver
    }
    
    public static GameManager Instance; 

    [Header("Timer Settings")]
    public float startTime = 60f;
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
            Destroy(gameObject);
    }

    void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (CurrentState != GameState.Playing)
            return;
        
        currentTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        
        if (currentTime <= 0f)
            EndGame();
    }
    
    public void StartGame()
    {
        currentTime = startTime;
        CurrentState = GameState.Playing;
    }

    public void EndGame()
    {
        CurrentState = GameState.GameOver;
        Debug.Log("Game Over!");
    }

    public void AddTime(float amount)
    {
        currentTime += amount;
    }

    public void SubtractTime(float amount)
    {
        currentTime -= amount;
        if (currentTime < 0) currentTime = 0;
    }
}
