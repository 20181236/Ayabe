using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Image fillImage;

    [SerializeField] private Transform buffIconContainer;
    [SerializeField] private GameObject buffIconPrefab;

    private float maxHealth;
    private float currentHealth;

    private RectTransform rectTransform;

    [SerializeField] private Vector3 offset = new Vector3(0, 5f, 0);

    private Dictionary<BuffGroup, GameObject> activeBuffIcons = new Dictionary<BuffGroup, GameObject>();

    private CharacterBase targetCharacter;
    private BuffManager boundBuffManager; // 현재 구독 중인 BuffManager 참조

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(CharacterBase character, float maxHp)
    {
        targetCharacter = character;
        target = character.transform;
        maxHealth = maxHp;
        currentHealth = maxHp;
        Debug.Log($"{character.name}의 HealthBar 생성됨");
        
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

        Vector3 worldPosition = target.position + offset;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z > 0)
        {
            rectTransform.position = screenPosition;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        else
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
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
        boundBuffManager.OnBuffAdded += OnBuffAddedHandler;
        boundBuffManager.OnBuffRemoved += OnBuffRemovedHandler;
    }

    // 이벤트 핸들러: 버프 추가
    private void OnBuffAddedHandler(Buff buff)
    {
        // buff.owner가 아닌, buff.receiver가 targetCharacter와 같은지 확인
        // 이 예시에서는 편의상 buff.owner를 receiver로 가정합니다.
        if (buff.owner != targetCharacter)
        {
            Debug.Log($"[HealthBarController] 다른 캐릭터 버프 무시: {buff.owner.name}");
            return;
        }

        AddBuffIcon(buff.group, buff.buffIcon);
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

    public void AddBuffIcon(BuffGroup group, Sprite iconSprite)
    {
        // 프리팹과 컨테이너가 제대로 할당되었는지 확인
        if (buffIconPrefab == null || buffIconContainer == null)
        {
            Debug.LogError("BuffIcon Prefab 또는 Container가 없습니다! Inspector를 확인하세요.");
            return;
        }

        // 아이콘이 생성되는지 확인
        GameObject icon = Instantiate(buffIconPrefab, buffIconContainer);
        if (icon == null)
        {
            Debug.LogError("버프 아이콘 프리팹 인스턴스화 실패!");
            return;
        }

        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition = Vector2.zero;

        // Image 컴포넌트가 있는지 확인
        var image = icon.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = iconSprite;
            Debug.Log("아이콘 생성 및 스프라이트 할당 성공: " + iconSprite.name);
        }
        else
        {
            Debug.LogError("인스턴스화된 프리팹에 Image 컴포넌트가 없습니다!");
        }

        activeBuffIcons[group] = icon;
    }

    public void RemoveBuffIcon(BuffGroup group)
    {
        Debug.Log($"[HealthBarController] RemoveBuffIcon 호출: {group}");

        if (!activeBuffIcons.ContainsKey(group))
        {
            Debug.LogWarning($"[HealthBarController] 제거할 아이콘이 없음: {group}");
            return;
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
