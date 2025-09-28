using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SlidePuzzle : MonoBehaviour
{
    // List of objects in the puzzle
    [SerializeField] private SlidePuzzleObject[] objects;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Vector2Int start;
    // Grid to track object positions
    private int[,] grid;
    private Dictionary<SlidePuzzleObject, int> objectToIndex = new();
    private float moveTimer = 0f;
    private Vector2Int origPos;
    private Vector2Int targetPos;
    private int currMovingIndex = -1;
    private const float MoveDuration = 1f;
    
    public bool CanMove => moveTimer <= 0f;

    public Vector2Int toGridPosition(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x - .5f) - start.x, Mathf.RoundToInt(pos.y - .5f) - start.y);
    }
    private bool isSafe(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight && grid[pos.x, pos.y] == 0;
    }

    private Vector3 toWorldPosition(Vector2Int pos)
    {
        return new Vector3(pos.x + start.x + .5f, pos.y + start.y + .5f, 0f);
    }

    void Awake()
    {
        grid = new int[gridWidth, gridHeight];
        // Initialize grid with objects
        for (int i = 0; i < objects.Length; i++)
        {
            SlidePuzzleObject obj = objects[i];
            Vector3 pos = obj.transform.position;
            Vector2Int gridPos = toGridPosition(pos);
            Assert.IsTrue(gridPos.x >= 0 && gridPos.x < gridWidth && gridPos.y >= 0 && gridPos.y < gridHeight, "Object out of bounds");
            grid[gridPos.x, gridPos.y] = i + 1; // Mark as occupied
            obj.gridPosition = gridPos;
            objectToIndex[obj] = i;
            obj.puzzle = this;
        }
    }

    public void MoveObject(SlidePuzzleObject obj)
    {
        if (!CanMove) return;
        Vector2Int objGridPos = obj.gridPosition;
        Vector2Int newGridPos = objGridPos + obj.direction;
        while (isSafe(newGridPos))
        {
            newGridPos += obj.direction;
        }
        newGridPos -= obj.direction; // Step back to last valid position
        if (newGridPos != objGridPos)
        {
            // Update grid
            grid[objGridPos.x, objGridPos.y] = 0;
            grid[newGridPos.x, newGridPos.y] = System.Array.IndexOf(objects, obj) + 1;
            // Move object
            obj.gridPosition = newGridPos;
            obj.canMove = false;
            moveTimer = MoveDuration;
            origPos = objGridPos;
            targetPos = newGridPos;
            currMovingIndex = System.Array.IndexOf(objects, obj);
        }
    }

    void FixedUpdate()
    {
        moveTimer -= Time.fixedDeltaTime;
        if (moveTimer < 0f) moveTimer = 0f;
        if (currMovingIndex >= 0)
        {
            SlidePuzzleObject obj = objects[currMovingIndex];
            float alpha = 1f - (moveTimer / MoveDuration);
            alpha = LeanTween.easeInOutSine(0, 1, alpha);
            obj.GetComponent<Rigidbody2D>().position = Vector3.Lerp(toWorldPosition(origPos), toWorldPosition(targetPos), alpha);
            if (moveTimer <= 0f)
            {
                currMovingIndex = -1;
                obj.GetComponent<Rigidbody2D>().position = toWorldPosition(targetPos);
                obj.canMove = true;
            }
        }
    }
}
