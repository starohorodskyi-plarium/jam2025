using System.Collections;
using TMPro;
using UnityEngine;
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
    private float timeBonus = 1f;
    private float timePenalty = 5f;
    private float showLabelDuration = 1f;
    private float currentTime;
    
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject timePenaltyLabel;
    public GameObject timeBonusLabel;
    
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
        
        StartCoroutine(ShowLabel(timeBonusLabel));
    }

    public void SubtractTime()
    {
        currentTime -= timePenalty;
        if (currentTime < 0) currentTime = 0;

        StartCoroutine(ShowLabel(timePenaltyLabel));
    }
    
    private IEnumerator ShowLabel(GameObject label)
    {
        if (label == null) 
            yield break;
        
        label.SetActive(true);
        yield return new WaitForSeconds(showLabelDuration);
        label.SetActive(false);
    }
}
