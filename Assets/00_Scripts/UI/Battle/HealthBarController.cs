using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Transform target;  // 체력바를 따라다닐 캐릭터 머리 위치
    [SerializeField] private Camera mainCamera; // 메인 카메라
    [SerializeField] private Image fillImage;   // 체력바 이미지 (Fill Amount 조절용)

    [SerializeField] private Transform buffIconContainer; // 버프 아이콘 부모
    [SerializeField] private GameObject buffIconPrefab;   // 버프 아이콘 프리팹

    private float maxHealth;
    private float currentHealth;

    private RectTransform rectTransform;

    [SerializeField] private Vector3 offset = new Vector3(0, 5f, 0);

    // 현재 표시 중인 버프 아이콘 목록
    private Dictionary<BuffGroup, GameObject> activeBuffIcons = new Dictionary<BuffGroup, GameObject>();

    private BuffManager boundBuffManager;

    private CharacterBase targetCharacter;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();
    }

    // 캐릭터와 체력 초기 설정
    public void Setup(CharacterBase targetCharacterBase, float maxHp)
    {
        targetCharacter = targetCharacterBase;
        target = targetCharacter.transform;
        maxHealth = maxHp;
        currentHealth = maxHp;
        UpdateHealthBar();
    }
    // 체력 갱신
    public void SetHealth(float currentHp)
    {
        currentHealth = currentHp;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (target == null)
            return;

        Vector3 worldPosition = target.position + offset;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z > 0) // 카메라 앞에 있을 때만 보이도록
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
        //// 체력바 위치를 캐릭터 머리 위치에 맞춤
        //transform.position = target.position+offset;

        //// 체력바가 항상 카메라 정면을 보도록 회전
        //transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }


    private void UpdateHealthBar()
    {
        if (fillImage != null && maxHealth > 0)
        {
            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    public void BindBuffManager(BuffManager manager)
    {
        if (boundBuffManager != null)
        {
            boundBuffManager.OnBuffAdded -= HandleBuffAdded;
            boundBuffManager.OnBuffRemoved -= HandleBuffRemoved;
        }

        boundBuffManager = manager;
        boundBuffManager.OnBuffAdded += HandleBuffAdded;
        boundBuffManager.OnBuffRemoved += HandleBuffRemoved;
    }

    private void HandleBuffAdded(BuffData data)
    {
        Debug.Log($"[HealthBarController] HandleBuffAdded 호출: {data.group}");
        AddBuffIcon(data.group, data.buffIcon);
    }

    private void HandleBuffRemoved(BuffData data)
    {
        Debug.Log($"[HealthBarController] HandleBuffRemoved 호출: {data.group}");
        RemoveBuffIcon(data.group);
    }

    public void AddBuffIcon(BuffGroup group, Sprite iconSprite)
    {
        if (activeBuffIcons.ContainsKey(group))
        {
            Debug.Log($"[BuffIcon] 이미 {group} 그룹 아이콘이 활성화 되어있음.");
            return;  // 이미 표시중인 그룹이면 중복 생성 안함
        }

        if (iconSprite == null)
        {
            Debug.LogWarning($"[BuffIcon] {group} 그룹의 아이콘 스프라이트가 할당되지 않았습니다.");
            return;
        }

        GameObject icon = Instantiate(buffIconPrefab, buffIconContainer);
        var imageComponent = icon.GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogWarning("[BuffIcon] 생성한 아이콘에 Image 컴포넌트가 없습니다.");
            return;
        }

        imageComponent.sprite = iconSprite;
        activeBuffIcons.Add(group, icon);

        Debug.Log($"[BuffIcon] {group} 그룹 아이콘 생성 완료, 스프라이트 이름: {iconSprite.name}");
    }
    public void RemoveBuffIcon(BuffGroup group)
    {
        if (!activeBuffIcons.ContainsKey(group))
            return;

        Destroy(activeBuffIcons[group]);
        activeBuffIcons.Remove(group);
    }
    private void OnDestroy()
    {

    }
}
