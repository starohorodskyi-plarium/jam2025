using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.SnailUI
{
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
        [FormerlySerializedAs("sText")] [SerializeField] private TextMeshProUGUI _sText;
        [FormerlySerializedAs("nText")] [SerializeField] private TextMeshProUGUI _nText;
        [FormerlySerializedAs("aText")] [SerializeField] private TextMeshProUGUI _aText;
        [FormerlySerializedAs("iText")] [SerializeField] private TextMeshProUGUI _iText;
        [FormerlySerializedAs("lText")] [SerializeField] private TextMeshProUGUI _lText;
        
        [Header("State")]
        [FormerlySerializedAs("unlocked")] [SerializeField] private bool[] _unlocked = new bool[5];
        
        private static readonly Color InactiveColor = Color.gray;

        private int MaxLetters => _unlocked?.Length ?? 5; // S, N, A, I, L
    
        public static Action<SnailLetter> LetterOpen;

        private void Start() => 
            UpdateLettersVisual();

        private void OnEnable() => 
            LetterOpen += Unlock;

        private void OnDisable() => 
            LetterOpen -= Unlock;

        private void OnValidate()
        {
            if (_unlocked is not { Length: 5 })
            {
                var newArr = new bool[5];
                if (_unlocked != null)
                {
                    var copy = Mathf.Min(_unlocked.Length, 5);
                    for (var i = 0; i < copy; i++) newArr[i] = _unlocked[i];
                }
                _unlocked = newArr;
            }
            UpdateLettersVisual();
        }

        private void UpdateLettersVisual()
        {
            var letters = GetLettersArray();
            for (var i = 0; i < letters.Length; i++)
            {
                var text = letters[i];
                if (text == null) 
                    continue;
                
                var isUnlocked = i >= 0 && i < MaxLetters && _unlocked[i];
                text.color = isUnlocked ? new Color(226, 84, 44, 255) : InactiveColor;
            }
        }

        private void SetLetter(SnailLetter letter, bool isUnlocked)
        {
            var index = (int)letter;
            if (index < 0 || index >= MaxLetters) 
                return;
            
            if (_unlocked[index] == isUnlocked) 
                return;
            
            _unlocked[index] = isUnlocked;
            UpdateLettersVisual();
        }

        private void Unlock(SnailLetter letter) => 
            SetLetter(letter, true);

        private TextMeshProUGUI[] GetLettersArray() => 
            new[] { _sText, _nText, _aText, _iText, _lText };
    }
}