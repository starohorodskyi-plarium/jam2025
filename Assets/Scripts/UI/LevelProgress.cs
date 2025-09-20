using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelProgress : MonoBehaviour
    {
        private const string TextFormat = "Demons killed: {0}/{1}";
        
        [SerializeField] private TMP_Text _text;

        private void Update()
        {
            var loadedLevel = GameManager.Instance.LoadedLevel;
            if (loadedLevel == null)
                return;
            
            _text.text = string.Format(TextFormat, loadedLevel.SpawnManager.EnemiesDefeatCount, loadedLevel.SpawnManager.BaddiesPerLevel);
        }
    }
}
