using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;

namespace Animations
{
    [RequireComponent(typeof(Button))]
    public class UiClickBoop : MonoBehaviour
    {
        [SerializeField] private Vector3 force;
        [SerializeField] [Range(0f,2f)] private float time;
        
        [SerializeField] private int vibrato;
        [SerializeField] [Range(0f,1f)] private float elasticity;
        
        [SerializeField] private UnityEvent clickSequence;
            
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
                .DOPunchScale(force, time, vibrato, elasticity)
                .OnComplete(() =>
                {
                    _button.transform.DOScale(_defaultScale.x, 0.3f).OnComplete(
                        () =>  clickSequence?.Invoke());
                });
        } 
        
        // Alarm 🩼🩼🩼
        public void ClickSequenceInvokeReference() =>
            clickSequence?.Invoke();
    }
}
