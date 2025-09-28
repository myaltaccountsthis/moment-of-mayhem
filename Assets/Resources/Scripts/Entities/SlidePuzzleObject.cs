using UnityEngine;

public class SlidePuzzleObject : InteractableEntity
{
    public Vector2Int gridPosition;
    public Vector2Int direction;
    public SlidePuzzle puzzle;
    
    public override void Interact(Player player)
    {
        puzzle.MoveObject(this);
    }
}
