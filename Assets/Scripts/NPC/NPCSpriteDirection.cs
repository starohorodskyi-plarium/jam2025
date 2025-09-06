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
    }
}


