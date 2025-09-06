using System;
using UnityEngine;
using TMPro;

public enum SnailLetter
{
    S = 0,
    N = 1,
    A = 2,
    I = 3,
    L = 4
}

public class SnailProgress : MonoBehaviour
{
    [Header("Letter Text References (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI sText;
    [SerializeField] private TextMeshProUGUI nText;
    [SerializeField] private TextMeshProUGUI aText;
    [SerializeField] private TextMeshProUGUI iText;
    [SerializeField] private TextMeshProUGUI lText;

    [Header("State")]
    [SerializeField] private bool[] unlocked = new bool[5];

    private static readonly Color ActiveColor = Color.white;
    private static readonly Color InactiveColor = Color.gray;

    private int MaxLetters => unlocked != null ? unlocked.Length : 5; // S, N, A, I, L
    
    public static Action<SnailLetter> LetterOpen;

    void Start()
    {
        UpdateLettersVisual();
    }

    private void OnEnable()
    {
        LetterOpen += Unlock;
    }

    private void OnDisable()
    {
        LetterOpen -= Unlock;
    }

    void OnValidate()
    {
        if (unlocked == null || unlocked.Length != 5)
        {
            var newArr = new bool[5];
            if (unlocked != null)
            {
                int copy = Mathf.Min(unlocked.Length, 5);
                for (int i = 0; i < copy; i++) newArr[i] = unlocked[i];
            }
            unlocked = newArr;
        }
        UpdateLettersVisual();
    }

    public void ResetProgress()
    {
        for (int i = 0; i < MaxLetters; i++) unlocked[i] = false;
        UpdateLettersVisual();
    }

    public void UpdateLettersVisual()
    {
        var letters = GetLettersArray();
        for (int i = 0; i < letters.Length; i++)
        {
            var text = letters[i];
            if (text == null) continue;
            bool isUnlocked = i >= 0 && i < MaxLetters && unlocked[i];
            text.color = isUnlocked ? ActiveColor : InactiveColor;
        }
    }

    public int GetUnlockedLettersCount()
    {
        int count = 0;
        for (int i = 0; i < MaxLetters; i++) if (unlocked[i]) count++;
        return count;
    }

    public void SetLetter(SnailLetter letter, bool isUnlocked)
    {
        int index = (int)letter;
        if (index < 0 || index >= MaxLetters) return;
        if (unlocked[index] == isUnlocked) return;
        unlocked[index] = isUnlocked;
        UpdateLettersVisual();
    }

    public void Unlock(SnailLetter letter)
    {
        SetLetter(letter, true);
    }

    public void Lock(SnailLetter letter)
    {
        SetLetter(letter, false);
    }

    public bool IsUnlocked(SnailLetter letter)
    {
        int index = (int)letter;
        if (index < 0 || index >= MaxLetters) return false;
        return unlocked[index];
    }

    private int FindNextLockedIndex()
    {
        for (int i = 0; i < MaxLetters; i++) if (!unlocked[i]) return i;
        return -1;
    }

    private TextMeshProUGUI[] GetLettersArray()
    {
        return new[] { sText, nText, aText, iText, lText };
    }
}
