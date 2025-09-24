using Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        public int LevelId;
        public SpawnManager SpawnManager;
        
        public UnityEvent OnLevelStarted;
    }
}
