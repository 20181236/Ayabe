using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Image fillImage;

    [SerializeField] private Transform buffIconContainer;
    [SerializeField] private GameObject buffIconPrefab;

    private float maxHealth;
    private float currentHealth;

    [SerializeField] private Vector3 offset = new Vector3(0, 10f, 0);

    private Dictionary<BuffGroup, GameObject> activeBuffIcons = new Dictionary<BuffGroup, GameObject>();
    private Dictionary<BuffGroup, Coroutine> flashingCoroutines = new Dictionary<BuffGroup, Coroutine>();

    private CharacterBase targetCharacter;
    private BuffManager boundBuffManager;

    public void Setup(CharacterBase character, float maxHp)
    {
        targetCharacter = character;

        // CharacterBase의 ObjectType 속성을 확인합니다.
        if (character.ObjectType == ObjectType.Playable)
        {
            // 이 캐릭터의 자식 오브젝트 중에서 "Hatch"를 찾습니다.
            Transform hatchTransform = character.transform.Find("Hatch");
            if (hatchTransform != null)
            {
                target = hatchTransform;
            }
            else
            {
                Debug.LogWarning($"'{character.name}'의 자식 중 'Hatch' 오브젝트를 찾을 수 없습니다. 캐릭터 자신을 타겟으로 설정합니다.");
                target = character.transform;
            }
        }
        else
        {
            // Playable 타입이 아니면(Enemy 등) 자기 자신을 타겟으로 설정합니다.
            target = character.transform;
        }

        maxHealth = maxHp;
        currentHealth = maxHp;
        Debug.Log($"{character.name}의 HealthBar 생성됨. 타겟: {target.name}");

        UpdateHealthBar();
    }

    public void SetHealth(float currentHp)
    {
        currentHealth = currentHp;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // 타겟이 사라지면 자기 자신 삭제
            return;
        }

        transform.position = target.position + offset;
    }

    private void UpdateHealthBar()
    {
        if (fillImage != null && maxHealth > 0)
        {
            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    // BuffManager 이벤트 바인딩
    public void BindBuffManager(BuffManager buffManager)
    {
        if (boundBuffManager != null)
        {
            boundBuffManager.OnBuffAdded -= OnBuffAddedHandler;
            boundBuffManager.OnBuffRemoved -= OnBuffRemovedHandler;
        }

        boundBuffManager = buffManager;
        boundBuffManager.OnBuffAdded += OnBuffAddedHandler;
        boundBuffManager.OnBuffRemoved += OnBuffRemovedHandler;
    }

    // 이벤트 핸들러: 버프 추가
    private void OnBuffAddedHandler(Buff buff, float duration)
    {
        if (buff.owner != targetCharacter)
        {
            return;
        }
        AddBuffIcon(buff.group, buff.buffIcon, duration);
    }

    // 이벤트 핸들러: 버프 제거
    private void OnBuffRemovedHandler(Buff buff)
    {
        if (buff.owner != targetCharacter)
        {
            return;
        }
        RemoveBuffIcon(buff.group);
    }

    public void AddBuffIcon(BuffGroup group, Sprite iconSprite, float duration)
    {
        if (buffIconPrefab == null || buffIconContainer == null)
        {
            Debug.LogError("BuffIcon Prefab 또는 Container가 없습니다! Inspector를 확인하세요.");
            return;
        }

        GameObject icon;
        if (activeBuffIcons.ContainsKey(group))
        {
            icon = activeBuffIcons[group];
            var image = icon.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = iconSprite;
            }
            if (flashingCoroutines.ContainsKey(group) && flashingCoroutines[group] != null)
            {
                StopCoroutine(flashingCoroutines[group]);
            }
        }
        else
        {
            icon = Instantiate(buffIconPrefab, buffIconContainer);
            var image = icon.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = iconSprite;
            }
            activeBuffIcons[group] = icon;
        }

        Coroutine flashingCoroutine = StartCoroutine(FlashBuffIconRoutine(group, icon.GetComponent<Image>(), duration));
        flashingCoroutines[group] = flashingCoroutine;
    }

    private IEnumerator FlashBuffIconRoutine(BuffGroup group, Image iconImage, float duration)
    {
        // duration이 2초보다 짧으면 깜빡이지 않도록 예외 처리
        if (duration <= 2.0f)
        {
            yield break;
        }

        float flashStartTime = duration - 2.0f;
        float flashInterval = 0.2f;

        yield return new WaitForSeconds(flashStartTime);

        float passedTime = 0f;
        while (passedTime < 2.0f) // 2초 동안만 깜빡이도록 수정
        {
            iconImage.color = new Color(1, 1, 1, 0.2f);
            yield return new WaitForSeconds(flashInterval);
            passedTime += flashInterval;

            // 코루틴이 도는 중 버프가 제거될 경우를 대비
            if (iconImage == null) yield break;

            iconImage.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(flashInterval);
            passedTime += flashInterval;
        }
    }

    public void RemoveBuffIcon(BuffGroup group)
    {
        if (!activeBuffIcons.ContainsKey(group))
        {
            return;
        }

        if (flashingCoroutines.ContainsKey(group) && flashingCoroutines[group] != null)
        {
            StopCoroutine(flashingCoroutines[group]);
            flashingCoroutines.Remove(group);
        }

        if (activeBuffIcons.ContainsKey(group) && activeBuffIcons[group] != null)
        {
            Destroy(activeBuffIcons[group]);
            activeBuffIcons.Remove(group);
        }
    }

    public Dictionary<BuffGroup, GameObject> GetActiveBuffIcons()
    {
        return activeBuffIcons;
    }

    private void OnDestroy()
    {
        if (boundBuffManager != null)
        {
            boundBuffManager.OnBuffAdded -= OnBuffAddedHandler;
            boundBuffManager.OnBuffRemoved -= OnBuffRemovedHandler;
        }
    }
}