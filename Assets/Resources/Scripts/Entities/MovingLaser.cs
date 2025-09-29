using UnityEngine;

public class MovingLaser : Entity, IInteractable
{
    [SerializeField] private float rotStart;
    [SerializeField] private float rotEnd;
    [SerializeField] private float rotTime;
    [SerializeField] private DamagePart laser;
    private ParticleSystem particles;
    private float rotTimer = 0f;
    private float slowTimer = 0f;
    private bool forward = true;

    protected override void Awake()
    {
        base.Awake();
        particles = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particles"), transform);
    }

    protected override void Start()
    {
        base.Start();
        transform.rotation = Quaternion.Euler(0f, 0f, rotStart);
    }

    protected override void Update()
    {
        base.Update();
        float dt = Time.deltaTime;
        if (slowTimer > 0f)
        {
            slowTimer -= dt;
            slowTimer = Mathf.Max(0f, slowTimer);
            if (slowTimer == 0f)
            {
                ParticleSystem.EmissionModule emission = particles.emission;
                emission.enabled = false;
                spriteRenderer.color = Color.white;
            }
            dt *= 0.25f;
        }
        if (forward)
        {
            rotTimer += dt;
            if (rotTimer >= rotTime)
            {
                rotTimer = rotTime;
                forward = false;
                laser.gameObject.SetActive(false);
            }
        }
        else
        {
            rotTimer -= dt;
            if (rotTimer <= 0f)
            {
                rotTimer = 0f;
                forward = true;
                laser.gameObject.SetActive(true);
            }
        }
        float alpha = rotTimer / rotTime;
        alpha = LeanTween.easeInOutSine(0, 1, alpha);
        float angle = Mathf.Lerp(rotStart, rotEnd, alpha);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void Interact(Player player)
    {
        slowTimer = 2f;
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = true;
        spriteRenderer.color = Color.lightGoldenRod;
    }
}
