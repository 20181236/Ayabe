using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public WeaponType weaponType = WeaponType.Bullet;
    public ObjectType ShooterType;
    public int damage;
    public float speed;
    public float rotateSpeed;
    public bool isExplosion;

    protected virtual void SetProjectileInfo()
    {
    }

    public virtual void OnHit(GameObject target)
    {
        //CharacterBase character = target.GetComponent<CharacterBase>();
        //if (character == null)
        //    return;
        if (!target.TryGetComponent<CharacterBase>(out var character))
            return;
        // 같은 타입이면 무시
        if (character.ObjectType == ShooterType)
            return;

        character.ApplyDamage(damage, isExplosion);
    }
}
