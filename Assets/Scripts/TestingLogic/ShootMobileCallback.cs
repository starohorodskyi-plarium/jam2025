using UnityEngine;
using UnityEngine.Events;

namespace TestingLogic
{
    public class ShootMobileCallback : MonoBehaviour
    {
        public UnityEvent OnShootPressed;
        
        public void InvokeShoot()
        {
            if (ShootCallback.IgnoreInputs)
                return;

            OnShootPressed?.Invoke();
        }
    }
}
