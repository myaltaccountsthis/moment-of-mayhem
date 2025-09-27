using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    private readonly HashSet<ReversibleEntity> reversibleEntities = new();
    public InputActionAsset inputActions;

    void Awake()
    {
        inputActions.Enable();
    }

    public void ReverseAll(int frames)
    {
        foreach (var entity in reversibleEntities)
        {
            // entity.Reverse(frames);
        }
    }
}
