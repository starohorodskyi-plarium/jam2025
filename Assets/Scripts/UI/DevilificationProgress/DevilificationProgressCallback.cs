using UnityEngine;
using UnityEngine.Serialization;

namespace UI.DevilificationProgress
{
    public class DevilificationProgressCallback : MonoBehaviour
    {
        [FormerlySerializedAs("progressValue")] [SerializeField] private float _progressValue;

        public void SetInstance() => 
            DevilificationProgress.OnSetInstant?.Invoke(_progressValue);

        public void SetSmooth() => 
            DevilificationProgress.OnSetSmooth?.Invoke(_progressValue);
    }
}