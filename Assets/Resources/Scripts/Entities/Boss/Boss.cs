using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : CollidableEntity
{
    [SerializeField] private BossAttack[] attacks;

    private BossAttack currentAttack;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        currentAttack = null;
    }

    protected override void Update()
    {
        base.Update();
    }
}
