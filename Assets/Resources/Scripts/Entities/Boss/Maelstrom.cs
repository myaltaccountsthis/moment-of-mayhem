
using System.Collections.Generic;
using UnityEngine;

class Maelstrom : BossAttack
{
    private ProjectileEntity bulletPrefab;
    private List<Transform> bullets;

    void Awake()
    {
        // bulletPrefab = Resources.Load<ProjectileEntity>("Prefabs/Entities/Projectiles/BossBullet");
        bullets = new List<Transform>();
    }

    public override void Execute()
    {
        // Implementation of the Maelstrom attack

    }
}