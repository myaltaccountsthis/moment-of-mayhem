using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelector : MonoBehaviour
{
    // to add: player, gamecontroller, selection box
    public SpriteRenderer selectionBox;
    public bool canSelectSelf = false;

    private const float InteractsPerSecond = 3f;

    private Player player;
    private GameController gameController;
    private InputAction clickAction;
    // private Material outlineMaterial;

    private Entity selectedEntity;
    // private Material selectedEntityOriginalMaterial;
    private float interactCooldown;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        clickAction = gameController.inputActions.FindAction("Attack");
        // outlineMaterial = Resources.Load<Material>("Materials/OutlineMaterial");

        clickAction.Enable();
    }

    void Start()
    {
        selectedEntity = null;
        // selectedEntityOriginalMaterial = null;
        interactCooldown = 0f;
    }

    void Update()
    {
        interactCooldown = Mathf.Max(0f, interactCooldown - Time.deltaTime);

        Entity oldEntity = selectedEntity;
        // Clear selection if on cooldown
        if (interactCooldown > 0f)
        {
            selectedEntity = null;
        }
        else
        {
            int interactableMask = canSelectSelf ? LayerMask.GetMask("Interactable", "Player") : LayerMask.GetMask("Interactable");
            // Perform raycast for interactable entities
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.CircleCast(mouseWorldPos, 0.25f, Vector2.zero, 0f, interactableMask);
            if (hit.collider != null && hit.collider.TryGetComponent(out Entity entity) && entity is IInteractable interactable)
            {
                // Check if the ray hit a collider
                selectedEntity = entity;

                if (clickAction.triggered && interactCooldown <= 0f)
                {
                    Debug.Log($"Interacting with {entity.name}");
                    interactable.Interact(player);
                    interactCooldown = 1f / InteractsPerSecond;
                }
            }
            else
            {
                selectedEntity = null;
            }
        }
        UpdateSelectionOutline(oldEntity);
        if (selectedEntity != null)
            selectionBox.transform.position = selectedEntity.transform.position;
    }

    // Selected entity should have an outline component, should be updated to new one
    void UpdateSelectionOutline(Entity oldEntity)
    {
        if (selectedEntity == null)
            selectionBox.enabled = false;

        if (selectedEntity == oldEntity)
        {
            return;
        }

        if (oldEntity != null)
        {
            // Deselect logic here, remove render
            if (oldEntity.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                // spriteRenderer.material = selectedEntityOriginalMaterial;
            }
        }

        // Update UI or other game elements to reflect the new selection
        // Implement select logic
        if (selectedEntity != null)
        {
            if (selectedEntity.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                selectionBox.enabled = true;
                // selectedEntityOriginalMaterial = spriteRenderer.material;
                // spriteRenderer.material = outlineMaterial;
            }
        }
    }
}
