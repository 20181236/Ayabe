using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using static BulletPoolManager;

public class Bullet : ProjectileBase

{
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo();
        weaponType = WeaponType.Bullet;
        isExplosion = false;
        damageMultiplier = 1.5f; // Bullet 고유 배율 설정 = 150%
    }

    //public void ReturnBullet(ProjectileBase projectile)
    //{
    //    BulletPoolManager.instance.ReturnBullet(this);
    //}
}


