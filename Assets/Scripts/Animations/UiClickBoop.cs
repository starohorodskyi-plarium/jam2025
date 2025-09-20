using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

namespace Animations
{
    [RequireComponent(typeof(Button))]
    public class UiClickBoop : MonoBehaviour
    {
        [FormerlySerializedAs("force")] [SerializeField] private Vector3 _force;
        [FormerlySerializedAs("time")] [SerializeField] [Range(0f,2f)] private float _time;
        
        [FormerlySerializedAs("vibrato")] [SerializeField] private int _vibrato;
        [FormerlySerializedAs("elasticity")] [SerializeField] [Range(0f,1f)] private float _elasticity;
        
        [FormerlySerializedAs("clickSequence")] [SerializeField] private UnityEvent _clickSequence;
            
        private Button _button;
        private Vector3 _defaultScale;

        private void Start()
        {
            _button = GetComponent<Button>();
            _defaultScale = transform.localScale;
        }

        public void Click()
        {
            _button.transform
                .DOPunchScale(_force, _time, _vibrato, _elasticity)
                .OnComplete(() =>
                {
                    _button.transform.DOScale(_defaultScale.x, 0.3f).OnComplete(
                        () =>  _clickSequence?.Invoke());
                });
        } 
        
        // Alarm 🩼🩼🩼
        public void ClickSequenceInvokeReference() =>
            _clickSequence?.Invoke();
    }
}
