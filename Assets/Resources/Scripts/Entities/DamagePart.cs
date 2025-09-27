using UnityEngine;

public class DamagePart : CollidableEntity
{
    public float damage = 10f;
    public virtual bool DestroyOnCollision => false;

    protected override void OnPlayerCollide(Player player)
    {
        player.TakeDamage(damage);
        if (DestroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}
