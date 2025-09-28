using System.Collections;
using UnityEngine;

public class FiringDevice : CollidableEntity, IInteractable
{
    public ProjectileEntity projectilePrefab;
    public Vector2 fixedDirection;
    public float mainDelay;
    public float slowedDelay;

    public bool isAiming;
    public Player target;
    // public AudioSource fireSound;

    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite shootingSprite;
    private Sprite normalSprite;

    private bool isSlowed = false;
    public float slowedDuration = 3f;
    private float currentDelay => isSlowed ? slowedDelay : mainDelay;
    private Coroutine coroutine;
    private ParticleSystem particles;

    protected bool IsSlowed => isSlowed;
    protected float CurrentDelay => currentDelay;

    protected override void Awake()
    {
        base.Awake();
        fixedDirection = fixedDirection.normalized;
        particles = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particles"), transform);
    }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
        coroutine = StartCoroutine(firing());
    }

    protected override void Update()
    {
        base.Update();
        if (enabled && isAiming && (target is not null))
        {
            transform.up = getTargetDirection();
        }
    }

    private Vector2 getTargetDirection()
    {
        return (target.transform.position - transform.position).normalized;
    }

    private IEnumerator WaitDynamic(System.Func<float> getDuration)
    {
        float progress = 0f;
        while (progress < 1f)
        {
            float dur = Mathf.Max(0.0001f, getDuration());
            progress += Time.deltaTime / dur;
            yield return null;
        }
    }

    IEnumerator firing()
    {
        while (true)
        {
            if (enabled)
                spriteRenderer.sprite = readySprite;

            yield return WaitDynamic(() => CurrentDelay / 4f);
            if (!enabled) continue;

            ProjectileEntity proj = Instantiate(projectilePrefab, GetProjectileStartPosition(), Quaternion.identity);
            Vector2 direction = isAiming && (target is not null)
                ? getTargetDirection()
                : fixedDirection;
            proj.transform.up = direction;

            // AudioSource audio = Instantiate(fireSound, transform.position, Quaternion.identity);
            // audio.Play();
            // Destroy(audio.gameObject, audio.clip.length);
            spriteRenderer.sprite = shootingSprite;

            yield return WaitDynamic(() => CurrentDelay / 2f);

            spriteRenderer.sprite = normalSprite;

            OnWait(CurrentDelay / 2f); 

            yield return WaitDynamic(() => CurrentDelay / 4f);
        }
    }

    protected virtual void OnWait(float seconds)
    {
        
    }

    protected virtual Vector3 GetProjectileStartPosition()
    {
        return transform.position;
    }

    public void Interact(Player player)
    {
        isSlowed = true;
        Invoke(nameof(ResetSpeed), slowedDuration);
        spriteRenderer.color = Color.lightGoldenRod;
        var emission = particles.emission;
        emission.enabled = true;
    }

    private void ResetSpeed()
    {
        isSlowed = false;
        spriteRenderer.color = Color.white;
        var emission = particles.emission;
        emission.enabled = false;
    }
}
