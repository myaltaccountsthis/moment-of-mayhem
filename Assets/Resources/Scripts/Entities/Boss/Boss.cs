using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : DamagePart
{
    private const int DifficultiesPerPhase = 3;

    enum BossPhase
    {
        Phase1,
        Phase2,
        Phase3
    }

    [SerializeField] private BossAttack[] attacks;
    private BossPhase phase = BossPhase.Phase1;
    private int currentDifficulty = 0;

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
        currentAttackIndex = 0;
        timeUntilNextAttack = 3f;
        currentAttack = attacks[0];
    }

    protected override void Update()
    {
        base.Update();

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
        if (phase == BossPhase.Phase1)
            phase = BossPhase.Phase2;
        else if (phase == BossPhase.Phase2)
            phase = BossPhase.Phase3;
        else if (phase == BossPhase.Phase3)
        {
            Debug.Log("Player wins");
        }

        currentDifficulty = (int)phase * DifficultiesPerPhase;
        Debug.Log("Boss advanced to phase: " + phase);
    }
}
