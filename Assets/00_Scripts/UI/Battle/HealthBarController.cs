using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private Dictionary<string, GameObject> activeBuffIcons = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();
    }

    // 캐릭터와 체력 초기 설정
    public void Setup(Transform targetTransform, float maxHp)
    {
        target = targetTransform;
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

    // 버프 아이콘 추가
    public void AddBuffIcon(string buffId, Sprite iconSprite)
    {
        if (activeBuffIcons.ContainsKey(buffId))
            return;

        GameObject icon = Instantiate(buffIconPrefab, buffIconContainer);
        icon.GetComponent<Image>().sprite = iconSprite;
        activeBuffIcons.Add(buffId, icon);
    }

    // 버프 아이콘 제거
    public void RemoveBuffIcon(string buffId)
    {
        if (!activeBuffIcons.ContainsKey(buffId))
            return;

        Destroy(activeBuffIcons[buffId]);
        activeBuffIcons.Remove(buffId);
    }
}
