using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using static BulletPoolManager;

public class Bullet : ProjectileBase
{
    protected override void SetProjectileInfo()
    {
        base.SetProjectileInfo(); weaponType = WeaponType.Bullet;
        damage = 10;
        isExplosion = false;
    }

    //public void ReturnBullet(ProjectileBase projectile)
    //{
    //    BulletPoolManager.instance.ReturnBullet(this);
    //}
}


