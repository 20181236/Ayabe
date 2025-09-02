using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;
using static UnityEditor.VersionControl.Asset;

public abstract class CharacterBase : MonoBehaviour
{
    public abstract ObjectType ObjectType { get; }

    [Header("Component References")]
    public Rigidbody _rigidbody;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    [Header("Base Stats")]
    public float baseMaxHealth;
    public float buffedMaxHealth;
    public float currentHealth;
    public float CurrentHealth => currentHealth;

    public float MaxHealth => baseMaxHealth + buffedMaxHealth;

    public float baseAttackPower;
    public float buffedAttackPower;
    public float AttackPower => baseAttackPower + buffedAttackPower;

    public float baseAttackRange;
    public float buffedAttackRange;
    public float AttackRange => baseAttackRange + buffedAttackRange;

    public float baseAttackSpeed; 
    public float buffedAttackSpeed;
    public float AttackSpeed => baseAttackSpeed + buffedAttackSpeed;

    public float baseHealPower;
    public float buffedHealPower;
    public float HealPower => baseHealPower + buffedHealPower;

    public float moveSpeed;

    [Header("Universal States")]
    public float distance; // 타겟과의 거리는 범용적으로 사용될 수 있습니다.
    public bool isAttacking; // 공격 애니메이션 재생 중임을 알리는 '잠금' 플래그
    public bool isDead;      // 모든 캐릭터는 죽을 수 있습니다.

    [Header("Cooldowns")]
    public float skillTimer;
    public float exSkillTimer;
    public bool readyBasicAttack;
    public bool readySkill;
    public bool readyExSkill;

    [Header("BasicAttack")]
    //public float basicAttackSpeed;
    public float basicAttackTimer;

    [Header("SkillCoolTime")]
    public float skillCoolTime;
    public float exSkillCoolTime;

    public float basicAttackCount;

    public bool isCreate;
    public bool isIdle;
    public bool isChase;
    public bool isAttack;

    public bool isBasicAttack;
    public bool isSkill;
    public bool isUsingSkill;

    public bool isExSkill;
    public bool isUsingExSkill;

    public bool checkInAttackRange;

    public Transform headTransform;
    public Transform bulletFirePoint;

    [Header("Health Bar")]
    [SerializeField] protected HealthBarController healthBarPrefab;
    protected HealthBarController healthBarInstance;

    [Header("Buff System")]
    public List<Buff> activeBuffs = new List<Buff>();
    [HideInInspector] public BuffManager buffManager;

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
    // BuffManager에게 버프 적용을 위임하는 메서드
    public void ApplyBuff(BuffData data, CharacterBase caster)
    {
        Debug.Log($"Applying Buff {data.buffId} to {name}, AttackPower={AttackPower}");

        if (data == null)
        {
            Debug.LogError($"[{name}] ApplyBuff 호출 시 BuffData가 null입니다.");
            return;
        }

        if (this.buffManager != null)
        {
            Debug.Log($"[{caster.name}]이(가) [{this.name}]에게 버프 적용: {data.buffId} / 값: {data.value} / 타입: {data.applyType}");
            // 이제 CharacterBase는 BuffManager에게 버프를 적용해달라고 요청만 함
            this.buffManager.ApplyBuff(data, this, caster);
        }
        else
        {
            Debug.LogWarning($"[{this.name}]의 BuffManager가 할당되지 않았습니다.");
        }
    }

    public void RecalculateBuffedStats()
    {
        buffedMaxHealth = 0f;
        buffedAttackPower = 0f;
        buffedAttackRange = 0f;
        buffedAttackSpeed = 0f;
        buffedHealPower = 0f;

        foreach (var buff in activeBuffs)
        {
            if (buff.applyType == BuffApplyType.Burst || buff.applyType == BuffApplyType.Continuous)
            {
                switch (buff.targetStat)
                {
                    case BuffStatType.AttackPower:
                        buffedAttackPower += baseAttackPower * buff.value;
                        break;
                    case BuffStatType.MaxHealth:
                        buffedMaxHealth += baseMaxHealth * buff.value;
                        break;
                    case BuffStatType.AttackRange:
                        buffedAttackRange += baseAttackRange * buff.value;
                        break;
                    case BuffStatType.AttackSpeed:
                        buffedAttackSpeed += baseAttackSpeed * buff.value;
                        break;
                    case BuffStatType.HealPower:
                        buffedHealPower += baseHealPower * buff.value;
                        break;
                }
            }
        }
        Debug.Log($"Recalculated Stats: AttackPower={AttackPower}, MaxHealth={MaxHealth}");
        currentHealth = Mathf.Min(currentHealth, MaxHealth);
    }
    protected virtual void OnBuffTick(Buff buff)
    {
        switch (buff.targetStat)
        {
            case BuffStatType.HealPower:
                Heal(baseHealPower * buff.value); // Tick마다 회복
                break;
        }
    }
    public void RemoveBuff(Buff buffToRemove)
    { 
        if (activeBuffs.Remove(buffToRemove))
            RecalculateBuffedStats();
    }
    public Vector3 GetCasterPosition()
    {
        return transform.position;
    }
}