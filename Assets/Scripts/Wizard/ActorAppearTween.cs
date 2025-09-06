using DG.Tweening;
using UnityEngine;

namespace Wizard
{
    public class ActorAppearTween : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _endPositionX;
        [SerializeField] private float _duration;

        private float _startPosition;

        private void Awake() => 
            _startPosition = _target.position.x;

        public void Show() => 
            _target.DOMoveX(_endPositionX, _duration);

        public void Hide() => 
            _target.DOMoveX(_startPosition, _duration);
    }
}
