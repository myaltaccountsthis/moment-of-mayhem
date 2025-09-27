using UnityEngine;

public abstract class CollidableEntity : ReversibleEntity
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnPlayerCollide(player);
        }
    }

    protected abstract void OnPlayerCollide(Player player);
}
