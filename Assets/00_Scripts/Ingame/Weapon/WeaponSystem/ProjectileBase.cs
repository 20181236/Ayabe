using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public WeaponType weaponType;
    public ObjectType ShooterType;
    public float damage;
    protected float damageMultiplier; // 무기 고유 배수
    public float speed;
    public float rotateSpeed;
    public bool isExplosion;

    public Transform target;

    public LayerMask targetMask;

    // 시전자 정보 (GameObject 또는 ID 등)
    protected GameObject shooter;

    // 시간 멈춤 무시 여부
    protected bool ignoreTimeScale ;

    protected virtual void Awake()
    {
        SetProjectileInfo();
        SetTargetMask();
    }

    protected virtual void SetProjectileInfo()
    {
    }

    public void SetDamageFromStat(float statValue)
    {
        damage = statValue * damageMultiplier;
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

    // 시전자 초기화 메서드 추가
    public void InitializeShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    // 시간 멈춤 무시 여부 설정
    public void SetIgnoreTimeScale(bool ignore)
    {
        ignoreTimeScale = ignore;
        Debug.Log($"ignoreTimeScale set to {ignoreTimeScale} on {gameObject.name}");
    }

    protected virtual void Update()
    {
        // 이동 처리 예시 (상속받은 자식에서 구체 구현 가능)
        MoveProjectile();
    }

    protected virtual void MoveProjectile()
    {
        // 예시: 앞으로 직진하는 투사체
        float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.position += transform.forward * speed * delta;
    }

    public virtual void OnHit(GameObject target)
    {
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
