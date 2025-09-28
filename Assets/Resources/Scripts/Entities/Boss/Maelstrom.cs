
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Maelstrom : BossAttack
{
    private const float DefaultLifetime = 100f;
    private static readonly WaitForSeconds _waitForSeconds2 = new(2);
    [SerializeField] private ProjectileEntity bulletPrefab;
    private List<ProjectileEntity> bullets;

    public float fireRatePerDirection = 2f;
    public int bulletCountPerDirection = 10;
    public int numDirections = 8;
    public float bulletSpeed = 5f;
    public float bulletDamage = 1f;
    public float rotationSpeed = -38f; // degrees per second
    public float minDistance = 15f, maxDistance = 30f;
    public float MaxDelayBeforeReverse => maxDistance / (bulletSpeed * 2f * SpeedFactor);

    void Start()
    {
        bullets = new List<ProjectileEntity>();
    }

    protected override IEnumerator Execute()
    {
        // Implementation of the Maelstrom attack
        int bulletsLeftPerDirection = Mathf.FloorToInt(bulletCountPerDirection * CountFactor);
        float rotation = 0f;
        float bulletSpeed = this.bulletSpeed * SpeedFactor;
        float rotationSpeed = this.rotationSpeed * SpeedFactor;


        int bulletsDone = 0;
        while (bulletsLeftPerDirection > 0)
        {
            float angleStep = 360f / numDirections;
            for (int i = 0; i < numDirections; i++)
            {
                float angle = i * angleStep + rotation;
                Vector3 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

                ProjectileEntity bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.damage = bulletDamage * DamageFactor;
                bullet.speed = bulletSpeed;
                bullet.transform.up = direction;
                bullet.lifetime = DefaultLifetime;
                bullets.Add(bullet);

                float timeUntilFreeze = Random.Range(minDistance, maxDistance) / bulletSpeed;
                LeanTween.delayedCall(timeUntilFreeze, () =>
                {
                    if (bullet != null)
                        bullet.SetSpeed(0);
                    bulletsDone++;
                });
            }
            bulletsLeftPerDirection--;

            rotation += rotationSpeed / fireRatePerDirection;
            yield return new WaitForSeconds(1f / fireRatePerDirection);
        }

        yield return new WaitUntil(() => bulletsDone >= bullets.Count); // Wait for a moment before reversing
        yield return _waitForSeconds2; // Wait for a moment before reversing

        float longestWait = 0f;
        // Reverse bullets
        foreach (var bullet in bullets)
        {
            if (bullet == null) continue; // Bullet might have been destroyed already if player reversed

            float myDelay = Random.Range(minDistance, maxDistance) / (bulletSpeed * 2f);
            float myLifetime = Vector2.Distance(bullet.transform.position, transform.position) / (bulletSpeed * 2f); // Adjust lifetime for reverse
            LeanTween.delayedCall(bullet.gameObject, myDelay, () =>
            {
                bullet.SetSpeed(-bulletSpeed * 2); // Reverse and double speed
                bullet.lifetime = myLifetime;
            });
            longestWait = Mathf.Max(longestWait, myLifetime + myDelay);
        }
        Debug.Log("Reversing all bullets with longest wait: " + longestWait);

        yield return new WaitForSeconds(longestWait); // Wait for all bullets to finish reversing

        bullets.Clear();
        IsAttacking = false;
    }
}