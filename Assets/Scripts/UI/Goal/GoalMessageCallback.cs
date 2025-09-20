using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Goal
{
    public class GoalMessageCallback : MonoBehaviour
    {
        [FormerlySerializedAs("goalMessage")] [SerializeField] private string _goalMessage;

        public void PushMessage() => 
            GoalMessage.UpdateGoalMessage?.Invoke(_goalMessage);
    }
}
