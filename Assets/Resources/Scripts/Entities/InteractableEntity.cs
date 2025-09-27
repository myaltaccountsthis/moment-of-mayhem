using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableEntity : Entity
{
    private Collider2D col;
    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
    }

    public virtual void Interact(Player player)
    {
        // Default interaction does nothing
    }
}
