using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossTimeBomb : CollidableEntity, IInteractable
{
    private const float InteractInterval = .01f;
    private const float InteractAcceleration = .6f;

    public float fuseTime = 15f;
    public float minSpeed = -3f, maxSpeed = 6.5f;
    public float acceleration = 2f;
    public float damage = 40f;
    public float explosionScale = 1f;

    private float timer;
    private float speed;

    [SerializeField] private DamagePart explosionPrefab;
    private Animator animator;
    private new Rigidbody2D rigidbody;
    private Transform player;
    private Transform boss;
    private Collider2D bossCollider;
    private bool primed;
    private float interactCooldown;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        boss = GameObject.Find("Boss").transform;
        bossCollider = boss.GetComponent<Collider2D>();

        timer = fuseTime;
        speed = 2f;
        animator.SetTrigger("Triggered");
        primed = false;
        interactCooldown = 0f;
    }

    protected override void Update()
    {
        base.Update();

        interactCooldown = Mathf.Max(0f, interactCooldown - Time.deltaTime);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (primed) return;

        timer -= Time.fixedDeltaTime;
        speed = Mathf.Clamp(speed + Time.fixedDeltaTime * acceleration, minSpeed, maxSpeed);

        if (timer <= 0f)
        {
            Prime();
        }
        else
        {
            Vector2 target = (speed < 0f ? boss : player).position;
            Vector2 direction = (target - rigidbody.position).normalized;
            rigidbody.MovePosition(rigidbody.position + Mathf.Abs(speed) * Time.fixedDeltaTime * direction);

            if (speed < 0f && collider.IsTouching(bossCollider))
            {
                Prime();
            }
        }
    }

    private void Prime()
    {
        if (primed) return;

        animator.SetTrigger("Primed");
        primed = true;
        gameObject.layer = LayerMask.GetMask("Ignore Raycast");
        LeanTween.scale(gameObject, transform.localScale * 1.5f, 0.3f).setEaseOutBack().setOnComplete(() =>
        {
            Explode();
        });
    }

    public void Interact(Player player)
    {
        if (primed) return;
        if (interactCooldown > 0f) return;

        interactCooldown = InteractInterval;
        speed = Mathf.Clamp(speed - InteractAcceleration, minSpeed, maxSpeed);
    }

    private void Explode()
    {
        // Add explosion effects here (e.g., damage player, play sound, etc.)
        Debug.Log("Boss Time Bomb Exploded!");
        animator.SetTrigger("Exploded");
        LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEaseInQuad().setOnComplete(() =>
        {
            // Create explosion
            DamagePart explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.damage = damage;
            explosion.transform.localScale = Vector3.zero;
            LeanTween.scale(explosion.gameObject, Vector3.one * explosionScale, 0.2f).setEaseOutQuad().setOnComplete(() =>
            {
                // Check if the boss was hit
                Collider2D explosionCollider = explosion.GetComponent<Collider2D>();
                if (explosionCollider.Distance(bossCollider).isOverlapped)
                {
                    Debug.Log("Explosion hit the boss, advancing phase.");
                    boss.GetComponent<Boss>().AdvancePhase();
                }

                explosion.GetComponent<ParticleSystem>().Stop();
                explosionCollider.enabled = false;
                LeanTween.delayedCall(0.5f, () => Destroy(explosion.gameObject));
            });
            Destroy(gameObject);
        });
    }

    protected override void OnPlayerCollide(Player player)
    {
        Prime();
    }
}