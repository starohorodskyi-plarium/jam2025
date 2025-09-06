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
    [SerializeField] private int unlockedLettersCount = 0;

    private static readonly Color ActiveColor = Color.white;
    private static readonly Color InactiveColor = Color.gray;

    private int MaxLetters => 5; // S, N, A, I, L

    void Start()
    {
        UpdateLettersVisual();
    }

    public int GetUnlockedLettersCount()
    {
        return unlockedLettersCount;
    }

    public void ResetProgress()
    {
        SetUnlockedLettersCount(0);
    }

    public void AddOne()
    {
        Add(1);
    }

    public void Add(int amount)
    {
        if (amount == 0) return;
        SetUnlockedLettersCount(unlockedLettersCount + amount);
    }

    public void UpdateLettersVisual()
    {
        var letters = GetLettersArray();
        for (int i = 0; i < letters.Length; i++)
        {
            var text = letters[i];
            if (text == null) continue;
            text.color = i < unlockedLettersCount ? ActiveColor : InactiveColor;
        }
    }

    private void SetUnlockedLettersCount(int value)
    {
        int clamped = Mathf.Clamp(value, 0, MaxLetters);
        if (clamped == unlockedLettersCount) return;
        unlockedLettersCount = clamped;
        UpdateLettersVisual();
    }

    private TMP_Text[] GetLettersArray()
    {
        return new[] { sText, nText, aText, iText, lText };
    }
}
