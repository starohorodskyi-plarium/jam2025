using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Animations
{
    public class WorldClickBoop : MonoBehaviour
    {
        [SerializeField] private Vector3 force;
        [SerializeField] [Range(0f,2f)] private float time;
        
        [SerializeField] private int vibrato;
        [SerializeField] [Range(0f,1f)] private float elasticity;
        
        [SerializeField] private UnityEvent clickSequence;
        
        private Vector3 _defaultScale;

        private void Start() => _defaultScale = transform.localScale;

        public void WorldClick()
        {
            transform
                .DOPunchScale(force, time, vibrato, elasticity)
                .OnComplete(() => transform.DOScale(_defaultScale.x,0.3f));
            
            clickSequence?.Invoke();
        } 
    }
}
