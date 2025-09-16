using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Transform target;
    //[SerializeField] private Camera mainCamera;
    [SerializeField] private Image fillImage;

    [SerializeField] private Transform buffIconContainer;
    [SerializeField] private GameObject buffIconPrefab;

    private float maxHealth;
    private float currentHealth;

    //private RectTransform rectTransform;

    [SerializeField] private Vector3 offset = new Vector3(0, 10f, 0);

    private Dictionary<BuffGroup, GameObject> activeBuffIcons = new Dictionary<BuffGroup, GameObject>();
    private Dictionary<BuffGroup, Coroutine> flashingCoroutines = new Dictionary<BuffGroup, Coroutine>();

    private CharacterBase targetCharacter;
    private BuffManager boundBuffManager; // 현재 구독 중인 BuffManager 참조

    private void Awake()
    {
        //if (mainCamera == null)
        //    mainCamera = Camera.main;

        //rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(CharacterBase character, float maxHp)
    {
        targetCharacter = character;

        // CharacterBase로부터 상속받은 ObjectType 속성을 확인합니다.
        if (character.ObjectType == ObjectType.Playable)
        {
            // 타입이 Playable이면 "Hatch" 오브젝트를 찾아서 타겟으로 설정합니다.
            GameObject hatchObject = GameObject.Find("Hatch");
            if (hatchObject != null)
            {
                target = hatchObject.transform;
            }
            else
            {
                Debug.LogWarning("'Hatch' 오브젝트를 찾을 수 없습니다. 캐릭터 자신을 타겟으로 설정합니다.");
                target = character.transform;
            }
        }
        else
        {
            // 타입이 Playable이 아니라면 (Enemy 등) 자기 자신을 타겟으로 설정합니다.
            target = character.transform;
        }

        // --- 이하 코드는 동일합니다 ---
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

        //Vector3 worldPosition = target.position + offset;
        //Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        //if (screenPosition.z > 0)
        //{
        //    rectTransform.position = screenPosition;
        //    if (!gameObject.activeSelf)
        //        gameObject.SetActive(true);
        //}
        ////else
        ////{
        ////    if (gameObject.activeSelf)
        ////        gameObject.SetActive(false);
        ////}
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
        // 기존 구독 해제 (중복 방지)
        if (boundBuffManager != null)
        {
            boundBuffManager.OnBuffAdded -= OnBuffAddedHandler;
            boundBuffManager.OnBuffRemoved -= OnBuffRemovedHandler;
        }

        boundBuffManager = buffManager;
        boundBuffManager.OnBuffAdded += OnBuffAddedHandler; // <-- 변경
        boundBuffManager.OnBuffRemoved += OnBuffRemovedHandler;
    }

    // 이벤트 핸들러: 버프 추가
    private void OnBuffAddedHandler(Buff buff, float duration)
    {
        if (buff.owner != targetCharacter)
        {
            Debug.Log($"[HealthBarController] 다른 캐릭터 버프 무시: {buff.owner.name}");
            return;
        }

        AddBuffIcon(buff.group, buff.buffIcon, duration);
    }

    // 이벤트 핸들러: 버프 제거
    private void OnBuffRemovedHandler(Buff buff)
    {
        Debug.Log($"[HealthBarController] 버프 제거 이벤트 감지: {buff.buffId} / 대상 캐릭터: {targetCharacter.name} / BuffGroup: {buff.group}");

        if (buff.owner != targetCharacter)
        {
            Debug.Log($"[HealthBarController] 다른 캐릭터 버프 제거 무시: {buff.owner.name}");
            return;
        }

        Debug.Log($"[HealthBarController] 아이콘 제거 시작: {buff.buffId}");
        RemoveBuffIcon(buff.group);
        Debug.Log($"[HealthBarController] 아이콘 제거 완료: {buff.buffId}");

        //if (buff.owner == targetCharacter)
        //{
        //    Debug.Log($"[HealthBarController] 이 캐릭터 버프 제거됨: {buff.buffId}");
        //    RemoveBuffIcon(buff.group);
        //}
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
            // 기존 아이콘이 있으면 깜박임 코루틴 갱신
            if (flashingCoroutines.ContainsKey(group) && flashingCoroutines[group] != null)
            {
                StopCoroutine(flashingCoroutines[group]);
            }
        }
        else
        {
            icon = Instantiate(buffIconPrefab, buffIconContainer);
            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            rt.anchoredPosition = Vector2.zero;

            var image = icon.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = iconSprite;
            }
            activeBuffIcons[group] = icon;
        }

        Debug.Log("아이콘 생성 및 스프라이트 할당 성공: " + iconSprite.name);
        // 깜박임 코루틴 시작
        Coroutine flashingCoroutine = StartCoroutine(FlashBuffIconRoutine(group, icon.GetComponent<Image>(), duration));
        flashingCoroutines[group] = flashingCoroutine;
    }

    // 아이콘 깜박임 코루틴을 새로 추가
    private IEnumerator FlashBuffIconRoutine(BuffGroup group, Image iconImage, float duration)
    {
        float flashStartTime = duration - 2.0f; // 종료 2초 전부터 깜박이기 시작
        float flashInterval = 0.2f;

        yield return new WaitForSeconds(flashStartTime);

        while (true)
        {
            iconImage.color = new Color(1, 1, 1, 0.2f); // 투명하게
            yield return new WaitForSeconds(flashInterval);
            iconImage.color = new Color(1, 1, 1, 1f); // 원래 색상으로
            yield return new WaitForSeconds(flashInterval);
        }
    }

    public void RemoveBuffIcon(BuffGroup group)
    {
        Debug.Log($"[HealthBarController] RemoveBuffIcon 호출: {group}");

        if (!activeBuffIcons.ContainsKey(group))
        {
            Debug.LogWarning($"[HealthBarController] 제거할 아이콘이 없음: {group}");
            return;
        }

        // 깜박임 코루틴 정지
        if (flashingCoroutines.ContainsKey(group) && flashingCoroutines[group] != null)
        {
            StopCoroutine(flashingCoroutines[group]);
            flashingCoroutines.Remove(group);
        }

        Destroy(activeBuffIcons[group]);
        activeBuffIcons.Remove(group);
        Debug.Log($"[HealthBarController] 아이콘 제거 완료: {group}");
    }
    public Dictionary<BuffGroup, GameObject> GetActiveBuffIcons()
    {
        return activeBuffIcons;
    }
    private void OnDestroy()
    {
        // 객체 파괴 시 구독 해제
        if (boundBuffManager != null)
        {
            boundBuffManager.OnBuffAdded -= OnBuffAddedHandler;
            boundBuffManager.OnBuffRemoved -= OnBuffRemovedHandler;
        }
    }
}
