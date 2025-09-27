using UnityEngine;

public class DamagePart : CollidableEntity
{
    public float damage = 10f;

    protected override void OnPlayerCollide(Player player)
    {
        player.TakeDamage(damage);
    }
}
