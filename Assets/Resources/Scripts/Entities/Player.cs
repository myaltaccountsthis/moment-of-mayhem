using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : ReversibleEntity
{
    private const float MoveSpeed = 5f;
    private const float MoveDamping = .05f;
    private const float MaxHealth = 100f;

    // Affects how fast or slow the player's health will drain over time
    public float healthDrainScale = 1f;

    private InputAction moveAction;
    private Vector2 inputVector, inputVelocity;
    // Health is "hp bar" in seconds, constantly drains
    private float health, bonusHealth;

    protected override void Awake()
    {
        base.Awake();

        moveAction = gameController.inputActions.FindAction("Move");
    }

    protected override void Start()
    {
        base.Start();
        health = MaxHealth;
        bonusHealth = 0f;
        inputVector = Vector2.zero;
        inputVelocity = Vector2.zero;
    }

    protected override void Update()
    {
        base.Update();

        TakeDamage(Time.deltaTime * healthDrainScale);
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
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void Heal(float amount)
    {
        // Implement healing logic here
        Debug.Log($"Player healed {amount} health.");
    }

    private void OnDeath()
    {
        // Implement death logic here
        Debug.Log("Player has died.");
        // Pause game, cover screen, reload scene
    }
}
