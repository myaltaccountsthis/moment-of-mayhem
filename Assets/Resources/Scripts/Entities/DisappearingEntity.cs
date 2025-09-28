using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DisappearingEntity : CollidableEntity, IInteractable
{
    private const float AnimationDuration = 0.7f;
    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool used = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    protected override void Update()
    {
        base.Update();

        if (!used)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = 0f;
        }
        float alpha = Mathf.Clamp01(timer / AnimationDuration);
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
        col.enabled = alpha > 0f;
    }

    public void Interact(Player player)
    {
        if (used)
        {
            return;
        }
        // Toggle visibility and collider state
        used = true;
        timer = AnimationDuration;
    }
}
