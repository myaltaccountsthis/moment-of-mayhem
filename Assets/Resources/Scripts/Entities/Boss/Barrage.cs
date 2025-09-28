
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Barrage : BossAttack
{
    private const float DamagePartFadeDuration = .4f;

    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private DamagePart damagePartPrefab;

    public int projectilesPerBurst = 10;
    public float timeBetweenBursts = 2f;
    public int numBursts = 3;
    public float fireRate = 10f;
    public float damagePartSize = 1f;
    public float projectileDuration = 1.5f;

    protected override IEnumerator Execute()
    {
        // Implementation of the Barrage attack
        int burstsLeft = Mathf.FloorToInt(numBursts * CountFactor);
        int projectilesPerBurst = Mathf.FloorToInt(this.projectilesPerBurst * CountFactor);
        float timeBetweenBursts = this.timeBetweenBursts / SpeedFactor;
        float projectileDuration = this.projectileDuration / SpeedFactor;
        float fireRate = this.fireRate * SpeedFactor;
        float damagePartSize = this.damagePartSize * DamageFactor;

        while (burstsLeft > 0)
        {
            int projectilesLeft = projectilesPerBurst;
            int projectilesDone = 0;

            while (projectilesLeft > 0)
            {
                // Random direction
                float angle = Random.Range(0f, 360f);
                // Random radius
                float radius = Random.Range(5f, 20f);
                Vector3 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                direction *= radius;

                Transform projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.up = direction;
                LeanTween.move(projectile.gameObject, transform.position + direction, projectileDuration).setEaseOutQuad().setOnComplete(() =>
                {
                    DamagePart damagePart = Instantiate(damagePartPrefab, projectile.position, Quaternion.identity);
                    damagePart.transform.localScale = new Vector3(damagePartSize, damagePartSize, 1f);

                    LeanTween.alpha(projectile.gameObject, 0f, DamagePartFadeDuration / SpeedFactor).setOnComplete(() =>
                    {
                        damagePart.GetComponent<Collider2D>().enabled = true;
                        LeanTween.alpha(damagePart.gameObject, 0f, DamagePartFadeDuration / SpeedFactor).setOnComplete(() =>
                        {
                            projectilesDone++;
                            Destroy(damagePart.gameObject);
                        });
                        Destroy(projectile.gameObject);
                    });
                });

                projectilesLeft--;
                yield return new WaitForSeconds(1f / fireRate);
            }

            yield return new WaitUntil(() => projectilesDone >= projectilesPerBurst);

            burstsLeft--;
            yield return new WaitForSeconds(timeBetweenBursts);
        }

        IsAttacking = false;
    }
}