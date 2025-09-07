using System;
using UI.Goal;
using UnityEngine;

namespace Gameplay.DevilMode
{
    public class DevilModeScenario : MonoBehaviour
    {
        [SerializeField] private DevilModePostProcessing _postProcessing;
        
        public static bool IsInDevilMode { get; private set; }
        
        public static Action ForceDevilMode;
        
        public static Action DevilModeActivated;
        public static Action DevilModeDeactivated;
        
        private const string DevilGoalMessage = "The game you played with cruel delight — now on yourself, it turns to bite.";

        private void OnEnable()
        {
            ForceDevilMode += EnterDevilMode;
        }

        private void OnDisable()
        {
            ForceDevilMode -= EnterDevilMode;
        }

        private void EnterDevilMode()
        {
            IsInDevilMode = true;
            _postProcessing.EnableDevilPostProcessing();
            GoalMessage.UpdateGoalMessage(DevilGoalMessage);	
            DevilModeActivated?.Invoke();
        }

        private void ExitDevilMode()
        {
            IsInDevilMode = false;
            _postProcessing.RevertDevilPostProcessing();
            DevilModeDeactivated?.Invoke();
        }
    }
}
