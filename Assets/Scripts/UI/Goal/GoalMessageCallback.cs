using UnityEngine;

namespace UI.Goal
{
    public class GoalMessageCallback : MonoBehaviour
    {
        [SerializeField] private string goalMessage;

        public void PushMessage()
        {
            GoalMessage.UpdateGoalMessage?.Invoke(goalMessage); 
        }
    }
}
