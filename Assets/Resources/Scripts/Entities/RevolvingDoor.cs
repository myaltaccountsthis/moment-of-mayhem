using UnityEngine;

public class RevolvingDoor : InteractableEntity
{
    private bool clockwise = false;
    private float rotationSpeed = 90f; // degrees per second

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
