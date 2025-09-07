using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Graphic))]
    public class AlphaTween : MonoBehaviour
    {
        [SerializeField] private float _from;
        [SerializeField] private float _to;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _runOnEnable;
        
        private Graphic _graphic;

        private void Awake() => 
            _graphic = GetComponent<Graphic>();

        private void OnEnable()
        {
            if (_runOnEnable)
                Tween();
        }

        public void Tween()
        {
            var color = _graphic.color;
            var to = new Color(color.r, color.g, color.b, _to);

            _graphic.DOColor(to, _duration);
        }
    }
}
