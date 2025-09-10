using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아래 자식으로 직선인지 커브인지 이런식으로 나눌것
//제네릭을 사용할것
public class ProjectileBase : MonoBehaviour
{
    public WeaponType weaponType;
    public ObjectType ShooterType;
    public float damage;
    protected float damageMultiplier;
    public float speed;
    public float rotateSpeed;
    public bool isExplosion;

    public Transform target;
    public LayerMask targetMask;

    protected GameObject shooter;
    protected bool ignoreTimeScale;

    private float lifeTime = 2f; // 총알 생존 시간
    private Coroutine lifeCycleCoroutine;

    protected virtual void Awake()
    {
        SetProjectileInfo();
        SetTargetMask();
    }

    protected virtual void OnEnable()
    {
        // 활성화될 때 라이프사이클 시작
        if (lifeCycleCoroutine != null)
            StopCoroutine(lifeCycleCoroutine);

        lifeCycleCoroutine = StartCoroutine(LifeCycle());
    }

    protected virtual void OnDisable()
    {
        // 비활성화 시 코루틴 정리
        if (lifeCycleCoroutine != null)
        {
            StopCoroutine(lifeCycleCoroutine);
            lifeCycleCoroutine = null;
        }
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    protected virtual void SetProjectileInfo() { }

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

    public void InitializeShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    public void SetIgnoreTimeScale(bool ignore)
    {
        ignoreTimeScale = ignore;
        Debug.Log($"ignoreTimeScale set to {ignoreTimeScale} on {gameObject.name}");
    }

    protected virtual void Update()
    {
        MoveProjectile();
    }

    protected virtual void MoveProjectile()
    {
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

        // 맞춘 후에도 풀 반환
        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        //나중에 제네릭으로 바꿀 것
        //BulletPoolManager.instance.ReturnBullet(this);
        BulletPoolManager.instance.ReturnBullet((Bullet)this);
    }
}
