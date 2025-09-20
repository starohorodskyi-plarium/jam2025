using UnityEngine;

namespace NPC
{
    public class NPCEnemySides : MonoBehaviour
    {
        [SerializeField] private bool _useMimicSide;
        [Space]
        [SerializeField] private SpriteRenderer _sprite;
        [Space]
        [SerializeField] private Sprite _baseSprite;
        [SerializeField] private Sprite _mimicSprite;
    
        private bool _initialFlip;
        private bool lastAppliedFlip;

        private void Start()
        {
            _initialFlip = _sprite.flipX;
            lastAppliedFlip = _initialFlip;
        }

        private void Update()
        {
            if (!lastAppliedFlip.Equals(_sprite.flipX)) 
                lastAppliedFlip = _sprite.flipX;
        }
    }
}
