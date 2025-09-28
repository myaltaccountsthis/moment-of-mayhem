using UnityEngine;

[RequireComponent(typeof(Collider2D), (typeof(Rigidbody2D)))]
public class SlidePuzzleObject : Entity, IInteractable
{
    public Vector2Int gridPosition;
    public Vector2Int direction;
    public SlidePuzzle puzzle;
    public bool canMove = true;

    public void Interact(Player player)
    {
        if (canMove)
        {
            puzzle.MoveObject(this);
        }
    }
}
