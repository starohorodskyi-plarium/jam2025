using System;
using UI.Goal;
using UnityEngine;

namespace Gameplay.DevilMode
{
    public class DevilModeScenario : MonoBehaviour
    {
        [SerializeField] private DevilModePostProcessing _postProcessing;
        
        public static Action DevilModeActivated;
        public static Action DevilModeDeactivated;
        
        private const string DevilGoalMessage = "The game you played with cruel delight — now on yourself, it turns to bite.";

        public void EnterDevilMode()
        {
            _postProcessing.EnableDevilPostProcessing();
            GoalMessage.UpdateGoalMessage(DevilGoalMessage);	
            DevilModeActivated?.Invoke();
        }

        public void ExitDevilMode()
        {
            _postProcessing.RevertDevilPostProcessing();
            DevilModeDeactivated?.Invoke();
        }
    }
}
