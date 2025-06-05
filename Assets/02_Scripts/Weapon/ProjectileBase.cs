using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public WeaponType weaponType;
    public ObjectType ShooterType;
    public float damage;
    public float speed;
    public float rotateSpeed;
    public bool isExplosion;

    public Transform target;

    public LayerMask targetMask;

    protected virtual void Awake()
    {
        SetProjectileInfo();
        SetTargetMask();
    }
    protected virtual void SetProjectileInfo()
    {
    }

    protected virtual void SetTargetMask()
    {
        if (ShooterType == ObjectType.Playable)
            targetMask = 1 << (int)GameLayerMask.Enemy;
        else if (ShooterType == ObjectType.Enemy)
            targetMask = 1 << (int)GameLayerMask.Playable;
        else
            targetMask = (1 << (int)GameLayerMask.Enemy) | (1 << (int)GameLayerMask.Playable);
    }

    public virtual void OnHit(GameObject target)
    {
        //CharacterBase character = target.GetComponent<CharacterBase>();
        //if (character == null)
        //    return;
        if (!target.TryGetComponent<CharacterBase>(out var character))
            return;
        if (character.ObjectType == ShooterType)
        {
            Debug.Log($"[TeamKill Prevented] Shooter ({ShooterType}) tried to hit same team target ({character.ObjectType}): {target.name}");
            return;
        }
        else
            character.ApplyDamage(damage, isExplosion);
    }
}
