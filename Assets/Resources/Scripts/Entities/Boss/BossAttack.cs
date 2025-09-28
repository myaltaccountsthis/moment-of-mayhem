using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    // Time in seconds after the attack is executed before the boss can attack again (endlag)
    public float cooldown;
    public bool IsAttacking { get; protected set; }

    // Difficulty scale, will affect projectile count, damage, speed, etc..
    // 0 = easy, 3 = medium, 6 = hard
    // Phase 1 = 0-2, Phase 2 = 3-5, Phase 3 = 6+
    // Difficulty increments by 1 after each attack cycle
    [HideInInspector]
    public int difficulty;
    public float SpeedFactor => Mathf.Pow(1.02f, difficulty) + (.04f * difficulty);
    public float DamageFactor => 1f + (.1f * difficulty);
    public float CountFactor => 1f + (.2f * difficulty);

    public void UseAttack()
    {
        IsAttacking = true;
        StartCoroutine(Execute());
    }
    protected abstract IEnumerator Execute();
}