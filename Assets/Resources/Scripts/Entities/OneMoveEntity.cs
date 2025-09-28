using UnityEngine;

public class OneMoveEntity : CollidableEntity, IInteractable
{
    [SerializeField] private Vector3 moveOffset;
    private const float AnimationDuration = 0.7f;
    private float timer = 0f;
    private Vector3 initialPosition;

    private bool used = false;

    protected override void Awake()
    {
        base.Awake();
        initialPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (!used)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = 0f;
        }
        float alpha = LeanTween.easeInOutSine(0, 1, 1 - Mathf.Clamp01(timer / AnimationDuration));
        transform.position = initialPosition + moveOffset * alpha;
    }

    public void Interact(Player player)
    {
        if (used)
        {
            return;
        }
        // Toggle visibility and collider state
        used = true;
        timer = AnimationDuration;
    }
}
