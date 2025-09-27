using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : ReversibleEntity
{
    private const float MoveSpeed = 5f;
    private const float MoveDamping = .05f;
    private const float MaxHealth = 100f;
    private const float InteractsPerSecond = 2f;

    // Affects how fast or slow the player's health will drain over time
    public float healthDrainScale = 1f;

    [SerializeField] private RectTransform healthBar;

    private InputAction moveAction, clickAction;
    private Vector2 inputVector, inputVelocity;
    // Health is "hp bar" in seconds, constantly drains
    private float health, bonusHealth;
    private float interactCooldown;

    protected override void Awake()
    {
        base.Awake();

        moveAction = gameController.inputActions.FindAction("Move");
        clickAction = gameController.inputActions.FindAction("Attack");
    }

    protected override void Start()
    {
        base.Start();
        health = MaxHealth;
        bonusHealth = 0f;
        inputVector = Vector2.zero;
        inputVelocity = Vector2.zero;

        interactCooldown = 0f;
        int interactableMask = LayerMask.GetMask("Interactable", "Player");
        clickAction.performed += ctx =>
        {
            // Check if player is on cooldown
            if (interactCooldown > 0f) return;
            interactCooldown = 1f / InteractsPerSecond;

            // Perform raycast for interactable entities
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.CircleCast(mouseWorldPos, 0.3f, Vector2.zero, 0f, interactableMask);
            if (hit.collider != null && hit.collider.TryGetComponent(out InteractableEntity entity))
            {
                Debug.Log("Interacting with " + entity.name);
                entity.Interact(this);
            }
        };

        UpdateHealthBar(false);
    }

    protected override void Update()
    {
        base.Update();

        TakeDamage(Time.deltaTime * healthDrainScale);
        interactCooldown = Math.Max(0f, interactCooldown - Time.deltaTime);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 targetInput = moveAction.ReadValue<Vector2>();
        if (targetInput.sqrMagnitude > 1f)
        {
            targetInput.Normalize();
        }
        inputVector = Vector2.SmoothDamp(inputVector, targetInput, ref inputVelocity, MoveDamping, 10, Time.fixedDeltaTime);
        if (inputVector.sqrMagnitude > 1f)
        {
            inputVector.Normalize();
        }
        rigidbody.MovePosition(rigidbody.position + MoveSpeed * Time.fixedDeltaTime * inputVector);
    }

    public void UpdateHealthBar(bool shouldTween = true)
    {
        if (shouldTween)
        {
            LeanTween.scaleX(healthBar.gameObject, health / MaxHealth, 0.2f).setEaseOutQuad();
        }
        else
        {
            healthBar.localScale = new Vector3(health / MaxHealth, 1f, 1f);
        }
    }

    public void TakeDamage(float amount)
    {
        // Implement health reduction logic here
        if (bonusHealth > 0)
        {
            float bonusUsed = Math.Min(bonusHealth, amount);
            bonusHealth -= bonusUsed;
            amount -= bonusUsed;
        }
        health = Math.Max(0, health - amount);
        UpdateHealthBar();
        if (health <= 0)
        {
            gameController.OnPlayerDeath();
        }
    }

    public void Heal(float amount)
    {
        // Implement healing logic here
        Debug.Log($"Player healed {amount} health.");
    }
}
