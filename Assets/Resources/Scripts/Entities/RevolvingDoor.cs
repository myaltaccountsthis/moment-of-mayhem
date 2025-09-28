using UnityEngine;

public class RevolvingDoor : CollidableEntity, IInteractable
{
    private bool clockwise = false;
    private float rotationSpeed = 90f; // degrees per second

    public void Interact(Player player)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    protected override void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        if (clockwise)
        {
            transform.Rotate(0f, 0f, -rotationAmount);
        }
        else
        {
            transform.Rotate(0f, 0f, rotationAmount);
        }
    }
}
