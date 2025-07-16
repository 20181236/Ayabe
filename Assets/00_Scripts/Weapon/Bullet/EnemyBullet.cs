using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        damage = 10;
        isExplosion = false;
    }
}