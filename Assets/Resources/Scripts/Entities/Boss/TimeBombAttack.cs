using System.Collections;
using UnityEngine;

public class TimeBombAttack : BossAttack
{
    [SerializeField] private BossTimeBomb timeBombPrefab;

    protected override IEnumerator Execute()
    {
        Vector3 spawnPosition = transform.position;
        BossTimeBomb timeBomb = Instantiate(timeBombPrefab, spawnPosition, Quaternion.identity);
        timeBomb.fuseTime /= SpeedFactor;
        timeBomb.acceleration *= SpeedFactor;
        timeBomb.damage *= DamageFactor;
        timeBomb.explosionScale *= SizeFactor;

        yield return new WaitUntil(() => timeBomb == null); // Wait for the bomb to explode

        IsAttacking = false;
    }
}
