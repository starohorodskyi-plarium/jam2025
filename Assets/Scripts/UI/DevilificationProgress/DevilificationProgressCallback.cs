using UnityEngine;

namespace UI.DevilificationProgress
{
    public class DevilificationProgressCallback : MonoBehaviour
    {
        [SerializeField] private float progressValue;

        public void SetInstance()
        {
            global::UI.DevilificationProgress.DevilificationProgress.OnSetInstant?.Invoke(progressValue);
        }

        public void SetSmooth()
        {
            global::UI.DevilificationProgress.DevilificationProgress.OnSetSmooth?.Invoke(progressValue);
        }
    }
}