using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Drone : ReversibleEntity
{
    [SerializeField] private float range;
    [SerializeField] private float followDistance;
    [SerializeField] private float speed;
    [SerializeField] private float rushSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int bulletsLeft;
    [SerializeField] private float fireRate;
    [SerializeField] private float meleeDamage = 20f;

    [SerializeField] private ProjectileEntity bulletPrefab;

    private Sprite defaultSprite;
    private Sprite pulseSprite;
    private Sprite shootingSprite;
    private Sprite rushingSprite;
    private Rigidbody2D rb;
    private Transform target;
    private Vector2 movementVelocity;
    private float fireCooldown;
    private float timer = 0f;
    private const float AnimCooldown = .5f;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        defaultSprite = spriteRenderer.sprite;
        pulseSprite = Resources.Load<Sprite>("Textures/DronePulse");
        shootingSprite = Resources.Load<Sprite>("Textures/DroneRush");
        rushingSprite = Resources.Load<Sprite>("Textures/DroneDive");

        if (rb.bodyType != RigidbodyType2D.Kinematic)
            Debug.LogWarning("Drone Rigidbody2D should be Kinematic for proper movement.");
        Debug.Assert(range >= followDistance, "Drone follow distance should be less than or equal to range.");
        Debug.Assert(bulletsLeft >= 0, "Drone bullets left should be non-negative");
        Debug.Assert(fireRate > 0f, "Drone fire rate should be positive.");
        Debug.Assert(bulletPrefab != null, "Drone bullet prefab is not assigned.");
    }

    protected override void Start()
    {
        base.Start();
        movementVelocity = Vector2.zero;
        fireCooldown = 0f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsReversing) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector2 targetPosition = target.position;
        if (bulletsLeft > 0 && Vector2.Distance(transform.position, target.position) <= range)
        {
            if (fireCooldown <= 0f)
            {
                Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

                if (Quaternion.Angle(transform.rotation, rotation) < 5f)
                {
                    // Shoot only if facing the player
                    spriteRenderer.sprite = shootingSprite;
                    timer = AnimCooldown;

                    Vector3 spawnLocation = transform.position + direction * bulletPrefab.GetComponent<Collider2D>().bounds.extents.y / 2f;
                    Instantiate(bulletPrefab, spawnLocation, rotation);
                    bulletsLeft--;
                    fireCooldown = 1f / fireRate;
                }
            }
            // Drone should stay a set distance away from the player while shooting
            targetPosition = target.position + (direction * followDistance);
        }


        if ((fireCooldown -= Time.fixedDeltaTime) <= 0f)
        {
            var rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), rotationSpeed * Time.fixedDeltaTime);
            if (bulletsLeft <= 0)
            {
                spriteRenderer.sprite = rushingSprite;
                // Rush the player when out of bullets
                rb.MovePositionAndRotation(transform.position + rushSpeed * Time.fixedDeltaTime * direction, rotation);
            }
            else
            {
                rb.MovePositionAndRotation(Vector2.SmoothDamp(transform.position, targetPosition, ref movementVelocity, .3f, speed, Time.fixedDeltaTime), rotation);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (IsReversing) return;
        timer -= Time.deltaTime;
        if (timer <= 0f && bulletsLeft > 0)
        {
            if (spriteRenderer.sprite != defaultSprite)
            {
                spriteRenderer.sprite = defaultSprite;
            }
            else
            {
                spriteRenderer.sprite = pulseSprite;
            }
            timer = AnimCooldown;
        }
    }

    protected override void OnPlayerCollide(Player player)
    {
        player.TakeDamage(meleeDamage);
        Destroy(gameObject);
    }
}