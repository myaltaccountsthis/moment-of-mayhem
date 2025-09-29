using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BossPhase
{
    Phase1,
    Phase2,
    Phase3,
    Defeated
}

[RequireComponent(typeof(SpriteRenderer))]
public class Boss : DamagePart
{
    private const int DifficultiesPerPhase = 3;

    [SerializeField] private BossAttack[] attacks;
    private BossPhase phase = BossPhase.Phase1;
    private int currentDifficulty = 0;

    private Sprite defaultSprite;
    [SerializeField] private Sprite[] chargingSprites;
    private int chargingSpriteIndex = 0;
    private const float chargingSpriteInterval = .3f;
    private float chargingSpriteTimer = 0f;

    private bool canAttack;
    private int currentAttackIndex;
    private float timeUntilNextAttack;
    private BossAttack currentAttack;

    private List<Drone> drones;
    [SerializeField] private Drone[] dronePrefabs;

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(attacks.Length > 0, "Boss must have at least one attack.");
        defaultSprite = spriteRenderer.sprite;
    }

    protected override void Start()
    {
        base.Start();

        canAttack = true;
        currentAttackIndex = 0;
        timeUntilNextAttack = 3f;
        currentAttack = attacks[0];
        drones = new();
    }

    protected override void Update()
    {
        base.Update();

        if (!canAttack) return;

        if (timeUntilNextAttack > 0)
        {
            if (!currentAttack.IsAttacking)
            {
                timeUntilNextAttack -= Time.deltaTime;
                if (timeUntilNextAttack < 2f)
                {
                    chargingSpriteTimer += Time.deltaTime;
                    if (chargingSpriteTimer >= chargingSpriteInterval)
                    {
                        chargingSpriteTimer = 0f;
                        chargingSpriteIndex = (chargingSpriteIndex + 1) % chargingSprites.Length;
                        spriteRenderer.sprite = chargingSprites[chargingSpriteIndex];
                    }
                }
                else
                    spriteRenderer.sprite = defaultSprite;
            }
            return;
        }

        currentAttack = attacks[currentAttackIndex];
        currentAttackIndex++;
        if (currentAttackIndex >= attacks.Length)
        {
            currentAttackIndex = 0;
            if (currentDifficulty < ((int)phase + 1) * DifficultiesPerPhase - 1)
                currentDifficulty++;
        }

        timeUntilNextAttack = 4f;
        Debug.Log("Using attack: " + currentAttack.GetType().Name);
        currentAttack.difficulty = currentDifficulty;
        currentAttack.UseAttack();
        spriteRenderer.sprite = currentAttack.bossSprite;
    }

    public void AdvancePhase()
    {
        canAttack = false;
        foreach (var drone in drones)
        {
            if (drone != null)
                Destroy(drone.gameObject);
        }

        if (phase == BossPhase.Phase1)
            phase = BossPhase.Phase2;
        else if (phase == BossPhase.Phase2)
            phase = BossPhase.Phase3;
        else if (phase == BossPhase.Phase3)
        {
            phase = BossPhase.Defeated;
            Debug.Log("Boss defeated!");
            gameController.OnBossDefeated();
            LeanTween.alpha(gameObject, 0f, 1f).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
            return;
        }

        int newPhase = (int)phase;
        currentDifficulty = newPhase * DifficultiesPerPhase;
        Debug.Log("Boss advanced to phase: " + phase);

        gameController.NextBossLevel(phase, () =>
        {
            foreach (var attack in attacks)
            {
                attack.IsAttacking = false;
            }

            canAttack = true;
            currentAttackIndex = 0;
            timeUntilNextAttack = 3f;
            currentAttack = attacks[0];

            int droneCount = newPhase * (newPhase + 1) / 2;
            float angleStep = 360f / droneCount;
            for (int i = 0; i < droneCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 offset = new(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                offset *= 2f;
                Drone drone = Instantiate(dronePrefabs[newPhase], transform.position + offset, Quaternion.identity);
                drones.Add(drone);
            }
        });
    }
}
