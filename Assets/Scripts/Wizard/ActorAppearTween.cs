using DG.Tweening;
using UnityEngine;

namespace Wizard
{
    public class ActorAppearTween : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private float _endPositionX;
        [SerializeField] private float _duration;

        private float _startPosition;

        private void Awake() => 
            _startPosition = _target.position.x;

        public void Show() => 
            _target.DOAnchorPosX(_endPositionX, _duration);

        public void Hide() => 
            _target.DOAnchorPosX(_startPosition, _duration);
    }
}
