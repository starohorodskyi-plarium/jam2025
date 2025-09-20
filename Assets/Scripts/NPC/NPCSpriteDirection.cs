using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class NPCSpriteDirection : MonoBehaviour
    {
        public enum Direction
        {
            Left,
            Right
        }

        [FormerlySerializedAs("spriteRenderer")] [SerializeField] private SpriteRenderer _spriteRenderer;
        [FormerlySerializedAs("direction")] [SerializeField] private Direction _direction = Direction.Right;
        [Space]
        [FormerlySerializedAs("shadow")][SerializeField] private Transform _shadow;

        private void Reset()
        {
            if (!_spriteRenderer)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            ApplyFlip();
        }

        private void OnValidate() => 
            ApplyFlip();

        public void GoLeft() => 
            SetDirection(Direction.Left);


        public void GoRight() => 
            SetDirection(Direction.Right);


        public void SetDirection(Direction newDirection)
        {
            _direction = newDirection;
            ApplyFlip();
        }

        private void ApplyFlip()
        {
            if (!_spriteRenderer)
                return;

            _spriteRenderer.flipX = _direction == Direction.Left;
        
            ApplyFlipShadow();
        }
    
        private void ApplyFlipShadow()
        {
            if (_shadow) 
                _shadow.localScale = new Vector3(
                    _direction == Direction.Left ? -1f : 1f, 
                    _shadow.localScale.y, 
                    _shadow.localScale.z);
        }
    }
}


