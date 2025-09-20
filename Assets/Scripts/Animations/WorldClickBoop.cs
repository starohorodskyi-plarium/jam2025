using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Animations
{
    public class WorldClickBoop : MonoBehaviour
    {
        [FormerlySerializedAs("force")] [SerializeField] private Vector3 _force;
        [FormerlySerializedAs("time")] [SerializeField] [Range(0f,2f)] private float _time;
        
        [FormerlySerializedAs("vibrato")] [SerializeField] private int _vibrato;
        [FormerlySerializedAs("elasticity")] [SerializeField] [Range(0f,1f)] private float _elasticity;
        
        [FormerlySerializedAs("clickSequence")] [SerializeField] private UnityEvent _clickSequence;
        
        private Vector3 _defaultScale;

        private void Start() => _defaultScale = transform.localScale;

        public void WorldClick()
        {
            transform
                .DOPunchScale(_force, _time, _vibrato, _elasticity)
                .OnComplete(() => transform.DOScale(_defaultScale.x,0.3f));
            
            _clickSequence?.Invoke();
        } 
    }
}
