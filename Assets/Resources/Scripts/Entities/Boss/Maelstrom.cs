
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

class Maelstrom : BossAttack
{
    private const float DefaultLifetime = 1000f;
    private static readonly WaitForSeconds _waitForSeconds2 = new(2);
    [SerializeField] private ProjectileEntity bulletPrefab;
    private List<ProjectileEntity> bullets;

    private const float FireRatePerDirection = 2f;
    private const int BulletCountPerDirection = 8;
    private const int NumDirections = 7;
    private const float BulletSpeed = 4f;
    private const float BulletDamage = 3f;
    private const float RotationSpeed = -41f; // degrees per second
    private const float MinDistance = 15f, MaxDistance = 20f;
    public float MaxDelayBeforeReverse => MaxDistance / (BulletSpeed * 2f * SpeedFactor);

    void Start()
    {
        bullets = new List<ProjectileEntity>();
    }

    protected override IEnumerator Execute()
    {
        // Implementation of the Maelstrom attack
        int bulletsLeftPerDirection = Mathf.FloorToInt(BulletCountPerDirection * CountFactor);
        float rotation = 0f;
        float bulletSpeed = BulletSpeed * SpeedFactor;
        float rotationSpeed = RotationSpeed * SpeedFactor;

        int bulletsDone = 0;
        while (bulletsLeftPerDirection > 0)
        {
            float angleStep = 360f / NumDirections;
            for (int i = 0; i < NumDirections; i++)
            {
                float angle = i * angleStep + rotation;
                Vector3 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

                ProjectileEntity bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.localScale *= SizeFactor;
                bullet.damage = BulletDamage * DamageFactor;
                bullet.speed = bulletSpeed;
                bullet.transform.up = direction;
                bullet.lifetime = DefaultLifetime;
                bullets.Add(bullet);

                float timeUntilFreeze = Random.Range(MinDistance, MaxDistance) / bulletSpeed;
                LeanTween.delayedCall(timeUntilFreeze, () =>
                {
                    if (bullet != null)
                        bullet.SetSpeed(0);
                    bulletsDone++;
                });
            }
            bulletsLeftPerDirection--;

            rotation += rotationSpeed / FireRatePerDirection;
            yield return new WaitForSeconds(1f / FireRatePerDirection);
        }

        yield return new WaitUntil(() => bulletsDone >= bullets.Count); // Wait for a moment before reversing
        yield return _waitForSeconds2; // Wait for a moment before reversing

        float longestWait = 0f;
        // Reverse bullets
        foreach (var bullet in bullets)
        {
            if (bullet == null) continue; // Bullet might have been destroyed already if player reversed

            float myDelay = Random.Range(0, MaxDistance) / (bulletSpeed * 2f);
            LeanTween.delayedCall(bullet.gameObject, myDelay, () =>
            {
                bullet.SetSpeed(-bulletSpeed * 2); // Reverse and double speed
                bullet.lifetime = 10;
            });
            longestWait = Mathf.Max(longestWait, myDelay);
        }

        yield return new WaitForSeconds(longestWait + 5f); // Wait for all bullets to finish reversing

        bullets.Clear();
        IsAttacking = false;
    }
}