using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : ReversibleEntity
{
    private const float MoveSpeed = 5f;

    private InputAction moveAction;
    private new Rigidbody2D rigidbody;

    protected override void Awake()
    {
        base.Awake();

        moveAction = gameController.inputActions.FindAction("Move");
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }
        rigidbody.MovePosition(rigidbody.position + MoveSpeed * Time.fixedDeltaTime * input);
    }
}
