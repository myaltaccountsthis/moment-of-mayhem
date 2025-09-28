using UnityEngine;

public class SlidePuzzleObject : CollidableEntity, IInteractable
{
    public Vector2Int gridPosition;
    public Vector2Int direction;
    public SlidePuzzle puzzle;
    
    public void Interact(Player player)
    {
        puzzle.MoveObject(this);
    }
}
