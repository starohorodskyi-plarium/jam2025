using System;
using UnityEngine;

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
    private bool isUsingMimicSide;

    private void Start()
    {
        _initialFlip = _sprite.flipX;
        lastAppliedFlip = _initialFlip;
        isUsingMimicSide = true;
    }

    private void Update()
    {
        if (lastAppliedFlip != _sprite.flipX)
        {
            lastAppliedFlip = _sprite.flipX;
            isUsingMimicSide = lastAppliedFlip == _initialFlip;
        }
    }
}
