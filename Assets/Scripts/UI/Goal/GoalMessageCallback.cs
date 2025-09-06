using UI.Goal;
using UnityEngine;

public class GoalMessageCallback : MonoBehaviour
{
    [SerializeField] private string goalMessage;

    public void PushMessage()
    {
       GoalMessage.UpdateGoalMessage?.Invoke(goalMessage); 
    }
}
