using UnityEngine;

public class InteractableEntity : Entity
{
    private Collider2D col;
    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogError("InteractableEntity requires a Collider2D component with isTrigger set to true.");
        }
    }

    public virtual void Interact()
    {
        // Default interaction does nothing
    }
}
