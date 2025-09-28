using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollidableEntity : Entity
{
    protected new Collider2D collider;

    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            OnPlayerCollide(player);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnPlayerCollide(player);
        }
    }

    protected virtual void OnPlayerCollide(Player player)
    {
        // Default collision does nothing
    }
}
