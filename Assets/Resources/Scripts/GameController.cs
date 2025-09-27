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

    public void AddEntity(ReversibleEntity entity)
    {
        reversibleEntities.Add(entity);
    }

    public void RemoveEntity(ReversibleEntity entity)
    {
        reversibleEntities.Remove(entity);
    }

    public void ReverseAll(int frames)
    {
        foreach (var entity in reversibleEntities)
        {
            entity.Reverse(frames);
        }
    }

    public void OnPlayerDeath()
    {
        // Pause game, cover screen, reload scene

        // Restart the scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
