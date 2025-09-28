using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Obstacle : Entity, IInteractable
{
    [SerializeField] private Sprite reverseSprite;
    private Collider2D col;
    private Rigidbody2D rb;

    private bool used = false;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Interact(Player player)
    {
        if (used) return;
        used = true;
        spriteRenderer.sprite = reverseSprite;
        col.enabled = false;
    }
}
