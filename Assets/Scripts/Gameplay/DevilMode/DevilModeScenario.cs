using System;
using UnityEngine;

namespace Gameplay.DevilMode
{
    public class DevilModeScenario : MonoBehaviour
    {
        public static Action DevilModeActivated;
        public static Action DevilModeDeactivated;

        public void EnterDevilMode()
        {
            DevilModeActivated?.Invoke();
        }

        public void ExitDevilMode()
        {
            DevilModeDeactivated?.Invoke();
        }
    }
}
