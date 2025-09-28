using UnityEngine;

public class ProjectileEntityData : ReversibleEntityData
{
    public float lifetime;

    public ProjectileEntityData(Vector3 pos, Quaternion rot, float lifetime) : base(pos, rot)
    {
        this.lifetime = lifetime;
    }

    public override void Apply(ReversibleEntity entity)
    {
        base.Apply(entity);
        ProjectileEntity projectile = entity as ProjectileEntity;
        projectile.lifetime = lifetime;
    }

    public override ReversibleEntityData Lerp(ReversibleEntityData other, float t)
    {
        ProjectileEntityData otherProj = other as ProjectileEntityData;
        Vector3 newPos = Vector3.Lerp(position, other.position, t);
        Quaternion newRot = Quaternion.Slerp(rotation, other.rotation, t);
        float newLifetime = Mathf.Lerp(lifetime, otherProj.lifetime, t);
        return new ProjectileEntityData(newPos, newRot, newLifetime);
    }
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileEntity : ReversibleEntity
{
    [SerializeField] private float speed = 5f;
    [SerializeField] internal float lifetime = 5f;
    [SerializeField] private float damage = 10f;
    public override bool DestroyableOnReverse => true;

    private Rigidbody2D rb;
    private bool hasCollided;

    protected override void Start()
    {
        base.Start();
        hasCollided = false;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
    }

    protected override void Update()
    {
        base.Update();

        if (IsReversing) return;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnPlayerCollide(Player player)
    {
        if (IsReversing) return;
        // Prevent multiple collisions
        if (hasCollided)
            return;
        hasCollided = true;

        player.TakeDamage(damage);
        Destroy(gameObject);
    }

    protected override ReversibleEntityData CaptureState()
    {
        return new ProjectileEntityData(transform.position, transform.rotation, lifetime);
    }
}