using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        weaponType = WeaponType.Bullet;
        isExplosion = false;
        damageMultiplier = 1.5f;
        speed = 20f;
    }
}