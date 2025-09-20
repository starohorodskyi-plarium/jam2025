using UnityEngine;

public class NPCSpriteDirection : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right
    }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Direction direction = Direction.Right;
    [Space]
    [SerializeField] private Transform shadow;

    private void Reset()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        ApplyFlip();
    }

    private void OnValidate()
    {
        ApplyFlip();
    }
    
    public void GoLeft()
    {
        SetDirection(Direction.Left);
    }

    
    public void GoRight()
    {
        SetDirection(Direction.Right);
    }


    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        ApplyFlip();
    }

    public void SetDirectionByVector(Vector2 movement)
    {
        if (movement.x == 0f)
            return;

        SetDirection(movement.x < 0f ? Direction.Left : Direction.Right);
    }

    private void ApplyFlip()
    {
        if (!spriteRenderer)
            return;

        spriteRenderer.flipX = direction == Direction.Left;
        
        ApplyFlipShadow();
    }
    
    private void ApplyFlipShadow()
    {
        if (shadow) 
            shadow.localScale = new Vector3(
                direction == Direction.Left ? -1f : 1f, 
                shadow.localScale.y, 
                shadow.localScale.z);
    }
}


