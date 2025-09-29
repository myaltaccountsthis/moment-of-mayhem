using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    public Sprite bossSprite;
    public bool IsAttacking { get; set; }

    // Difficulty scale, will affect projectile count, damage, speed, etc..
    // 0 = easy, 3 = medium, 6 = hard
    // Phase 1 = 0-2, Phase 2 = 3-5, Phase 3 = 6+
    // Difficulty increments by 1 after each attack cycle
    [HideInInspector]
    public int difficulty;
    public float SpeedFactor => Mathf.Pow(1.03f, difficulty) + (.06f * difficulty);
    public float DamageFactor => 1f + (.1f * difficulty);
    public float CountFactor => 1f + (.12f * difficulty);
    public float SizeFactor => 1f + (.07f * difficulty);

    public void UseAttack()
    {
        IsAttacking = true;
        StartCoroutine(Execute());
    }
    protected abstract IEnumerator Execute();
}