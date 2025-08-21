using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public abstract class CharacterBase : MonoBehaviour
{
    public abstract ObjectType ObjectType { get; }

    [Header("Component References")]
    public Rigidbody _rigidbody;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public float baseMaxHealth;
    public float buffedMaxHealth;
    public float currentHealth;
    public float MaxHealth => baseMaxHealth + buffedMaxHealth;
    public float CurrentHealth => currentHealth;
    public float baseAttackPower;
    public float buffedAttackPower;
    public float AttackPower => baseAttackPower + buffedAttackPower;
    public float baseAttackRange;
    public float buffedAttackRange;
    public float AttackRange => baseAttackRange + buffedAttackRange;
    public float baseAttackInterval;
    public float buffedAttackInterval;
    public float AttackInterval => baseAttackInterval + buffedAttackInterval;
    public float baseHealPower;
    public float buffedHealPower;
    public float HealPower => baseHealPower + buffedHealPower;
    public float basicAttackTimer;
    public float basicAttackCount;
    public float skillInterval;
    public float skillTimer;
    public float exSkillInterval;
    public float exSkillTimer;
    public float moveSpeed;
    public float distance;
    public bool isCreate;
    public bool isIdle;
    public bool isChase;
    public bool isAttack;
    public bool isAttacking;
    public bool isBasicAttack;
    public bool isSkill;
    public bool isUsingSkill;
    public bool isExSkill;
    public bool isUsingExSkill;
    public bool isDead;
    public bool checkInAttackRange;
    public bool readyBasicAttack;
    public bool readySkill;
    public bool readyExSkill;

    public Transform headTransform;
    public Transform bulletFirePoint;
    // protected CharacterBase currentTarget; // 자식 클래스에서 선언

    [Header("Health Bar")]
    [SerializeField] protected HealthBarController healthBarPrefab;
    protected HealthBarController healthBarInstance;

    [Header("Buff System")]
    public List<Buff> activeBuffs = new List<Buff>();
    [HideInInspector] public BuffManager buffManager;

    // 이 클래스에서는 상태 Enum을 제거하고 자식 클래스에서 직접 관리합니다.
    // public CharacterState currentState;


    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        buffManager = GetComponent<BuffManager>();
        Initialize();
    }

    protected virtual void Start()
    {
        InitHealthBar();
    }

    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }

    protected virtual void Initialize()
    {
        currentHealth = MaxHealth;
        isCreate = true;
        readyBasicAttack = true;
    }

    protected void InitHealthBar()
    {
        if (healthBarPrefab != null && healthBarInstance == null)
        {
            GameObject canvas = GameObject.Find("HPBarCanvas");
            if (canvas == null)
            {
                canvas = new GameObject("HPBarCanvas");
                Canvas c = canvas.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
            }
            var instance = Instantiate(healthBarPrefab, canvas.transform);
            instance.Setup(this, MaxHealth);
            BuffManager myBuffManager = GetComponent<BuffManager>();
            if (myBuffManager != null)
            {
                instance.BindBuffManager(myBuffManager);
            }
            else
            {
                Debug.LogWarning($"{name}에 BuffManager가 없음. HealthBar의 버프 아이콘은 표시되지 않음.");
            }
            healthBarInstance = instance;
        }
    }

    protected virtual void CoolTime() { }
    protected virtual void AttackThinking() { }
    protected virtual void BasicAttack() { }
    protected virtual void ShootBulletAtTarget() { }
    protected virtual void Skill() { }
    protected virtual void ExSkill() { }

    public abstract void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null);

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        if (healthBarInstance != null) healthBarInstance.SetHealth(currentHealth);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        isChase = false;
        animator.SetTrigger("doDie");
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
            healthBarInstance = null;
        }
        OnDestroyed();
    }

    protected virtual void OnDestroyed()
    {
        Destroy(gameObject, 1.8f);
    }

    // ... Buff 관련 메서드들은 동일 ...
    public void ApplyBuff(BuffData data, CharacterBase caster)
    {
        if (this.buffManager != null)
        {
            Debug.Log($"[{caster.name}]이(가) [{this.name}]에게 버프 적용: {data.buffId} / 값: {data.value} / 타입: {data.applyType}");
            this.buffManager.ApplyBuff(data, this, caster);
        }
        else
        {
            Debug.LogWarning($"[{this.name}]의 BuffManager가 할당되지 않았습니다.");
        }
    }
    public void RemoveBuff(Buff buffToRemove)
    {
        if (activeBuffs.Remove(buffToRemove)) RecalculateBuffedStats();
    }
    public void RecalculateBuffedStats()
    {
        buffedMaxHealth = 0f;
        buffedAttackPower = 0f;
        buffedAttackRange = 0f;
        buffedAttackInterval = 0f;
        buffedHealPower = 0f;
        foreach (var buff in activeBuffs)
        {
            float buffValue = buff.value;
            switch (buff.applyType)
            {
                case BuffApplyType.Burst:
                case BuffApplyType.Tick:
                    switch (buff.targetStat)
                    {
                        case BuffStatType.MaxHealth: buffedMaxHealth += baseMaxHealth * buffValue; break;
                        case BuffStatType.AttackPower: buffedAttackPower += baseAttackPower * buffValue; break;
                        case BuffStatType.AttackRange: buffedAttackRange += baseAttackRange * buffValue; break;
                        case BuffStatType.AttackInterval: buffedAttackInterval += baseAttackInterval * buffValue; break;
                        case BuffStatType.HealPower: buffedHealPower += baseHealPower * buffValue; break;
                    }
                    break;
                case BuffApplyType.Continuous:
                    switch (buff.targetStat)
                    {
                        case BuffStatType.MaxHealth: buffedMaxHealth += baseMaxHealth * buffValue; break;
                        case BuffStatType.AttackPower: buffedAttackPower += baseAttackPower * buffValue; break;
                        case BuffStatType.AttackRange: buffedAttackRange += baseAttackRange * buffValue; break;
                        case BuffStatType.AttackInterval: buffedAttackInterval += baseAttackInterval * buffValue; break;
                        case BuffStatType.HealPower: buffedHealPower += baseHealPower * buffValue; break;
                    }
                    break;
            }
        }
        Debug.Log($"Recalculated Stats: AttackPower={AttackPower}, MaxHealth={MaxHealth}");
        currentHealth = Mathf.Min(currentHealth, MaxHealth);
    }
    private IEnumerator BuffRoutine(Buff buff)
    {
        float elapsed = 0f;
        Debug.Log($"BuffRoutine 시작: {buff.targetStat}, applyType: {buff.applyType}, duration: {buff.duration}, tickInterval: {buff.tickInterval}");
        if (buff.applyType == BuffApplyType.Tick)
        {
            float interval = Mathf.Max(buff.tickInterval, 0.1f);
            while (elapsed < buff.duration)
            {
                Debug.Log($"BuffRoutine 진행중: elapsed={elapsed}");
                yield return new WaitForSeconds(interval);
                OnBuffTick(buff);
                Debug.Log($"OnBuffTick 호출됨: {buff.targetStat}");
                elapsed += interval;
            }
        }
        else if (buff.applyType == BuffApplyType.Continuous)
        {
            yield return new WaitForSeconds(buff.duration);
        }
        else
        {
            OnBuffTick(buff);
            Debug.Log($"OnBuffTick 호출됨 (Burst 타입): {buff.targetStat}");
            yield return new WaitForSeconds(buff.duration);
        }
        Debug.Log($"BuffRoutine 종료: {buff.targetStat}");
        activeBuffs.Remove(buff);
        RecalculateBuffedStats();
    }
    protected virtual void OnBuffTick(Buff buff)
    {
        switch (buff.targetStat)
        {
            case BuffStatType.HealPower: Heal(baseHealPower * buff.value); break;
        }
    }
    public Vector3 GetCasterPosition()
    {
        return transform.position;
    }
}