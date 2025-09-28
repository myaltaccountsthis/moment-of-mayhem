using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class RevolvingDoor : Entity, IInteractable
{
    private bool clockwise = false;
    private float rotationSpeed = 90f; // degrees per second

    protected override void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Rigidbody2D>().angularVelocity = 90f;
    }

    public void Interact(Player player)
    {
        clockwise = !clockwise;
        GetComponent<Rigidbody2D>().angularVelocity = clockwise ? -rotationSpeed : rotationSpeed;
    }
}
