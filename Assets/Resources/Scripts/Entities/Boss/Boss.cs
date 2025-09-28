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

public class Boss : DamagePart
{
    private const int DifficultiesPerPhase = 3;

    [SerializeField] private BossAttack[] attacks;
    private BossPhase phase = BossPhase.Phase1;
    private int currentDifficulty = 0;

    private bool canAttack;
    private int currentAttackIndex;
    private float timeUntilNextAttack;
    private BossAttack currentAttack;

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(attacks.Length > 0, "Boss must have at least one attack.");
    }

    protected override void Start()
    {
        base.Start();

        canAttack = true;
        currentAttackIndex = 0;
        timeUntilNextAttack = 3f;
        currentAttack = attacks[0];
    }

    protected override void Update()
    {
        base.Update();

        if (!canAttack) return;

        if (timeUntilNextAttack > 0)
        {
            if (!currentAttack.IsAttacking)
                timeUntilNextAttack -= Time.deltaTime;
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

        timeUntilNextAttack = currentAttack.cooldown;
        Debug.Log("Using attack: " + currentAttack.GetType().Name);
        currentAttack.difficulty = currentDifficulty;
        currentAttack.UseAttack();
    }

    public void AdvancePhase()
    {
        canAttack = false;
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
        });
    }
}
