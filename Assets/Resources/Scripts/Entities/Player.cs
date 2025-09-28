using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : ReversibleEntity
{
    private const float MoveSpeed = 5f;
    private const float MoveDamping = .05f;
    private const float MaxHealth = 100f;

    // Affects how fast or slow the player's health will drain over time
    public float healthDrainScale = 1f;
    public bool isInvincible = false;

    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Sprite deathSprite;

    private Animator animator;
    private InputAction moveAction;
    private Vector2 inputVector, inputVelocity;
    // Health is "hp bar" in seconds, constantly drains
    private float health, bonusHealth;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        moveAction = gameController.inputActions.FindAction("Move");
    }

    protected override void Start()
    {
        base.Start();
        health = MaxHealth;
        bonusHealth = 0f;
        inputVector = Vector2.zero;
        inputVelocity = Vector2.zero;

        UpdateHealthBar(false);
    }

    protected override void Update()
    {
        base.Update();

        TakeDamage(Time.deltaTime * healthDrainScale, 0);
        if (isInvincible && !IsReversing)
        {
            isInvincible = false;
        }
    }

    void LateUpdate()
    {
        UpdateHealthBar();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsReversing) return;

        Vector2 targetInput = moveAction.ReadValue<Vector2>();
        if (targetInput.sqrMagnitude > 1f)
        {
            targetInput.Normalize();
        }
        inputVector = Vector2.SmoothDamp(inputVector, targetInput, ref inputVelocity, MoveDamping, 20, Time.fixedDeltaTime * 2);
        if (inputVector.sqrMagnitude > 1f)
        {
            inputVector.Normalize();
        }
        animator.SetFloat("VelocityX", inputVector.x);
        animator.SetFloat("VelocityY", inputVector.y);
        rigidbody.MovePosition(rigidbody.position + MoveSpeed * Time.fixedDeltaTime * inputVector);
    }

    private Vector3 healthBarVelocity = Vector3.zero;
    public void UpdateHealthBar(bool shouldEase = true)
    {
        Vector3 targetScale = new(health / MaxHealth, 1f, 1f);
        if (shouldEase)
        {
            healthBar.localScale = Vector3.SmoothDamp(healthBar.localScale, targetScale, ref healthBarVelocity, .1f, Mathf.Infinity, Time.deltaTime);
        }
        else
        {
            healthBar.localScale = targetScale;
        }
    }

    public void TakeDamage(float amount, int reverseFrames = 90)
    {
        if (IsReversing) return;

        // Implement health reduction logic here
        if (bonusHealth > 0)
        {
            float bonusUsed = Mathf.Min(bonusHealth, amount);
            bonusHealth -= bonusUsed;
            amount -= bonusUsed;
        }
        health = Mathf.Max(0, health - amount);
        if (health <= 0)
        {
            UpdateHealthBar(false);
            animator.enabled = false;
            spriteRenderer.sprite = deathSprite;
            rigidbody.simulated = false;
            gameController.OnPlayerDeath();
        }
        else
        {
            isInvincible = true;
            if (reverseFrames > 0)
                gameController.ReverseAll(reverseFrames);
        }
    }

    public void Heal(float amount)
    {
        // Implement healing logic here
        Debug.Log($"Player healed {amount} health.");
    }
}
