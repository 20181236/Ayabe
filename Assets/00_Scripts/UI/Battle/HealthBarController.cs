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

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();
    }

    //public void Setup(PlayableBase character, float maxHp)
    public void Setup(CharacterBase character, float maxHp)
    {
        targetCharacter = character;
        target = character.transform;
        maxHealth = maxHp;
        currentHealth = maxHp;
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
            return;

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

    public void BindBuffManager(BuffManager buffManager)
    {
        buffManager.OnBuffAdded += (buff) =>
        {
            Debug.Log($"[HealthBarController] 버프 추가 이벤트 감지: {buff.buffId} / 대상 캐릭터: {targetCharacter.name}");
            if (buff.owner == targetCharacter)
            {
                Debug.Log($"[HealthBarController] 이 캐릭터에게 버프 적용됨: {buff.buffId}");
                AddBuffIcon(buff.group, buff.buffIcon);
            }
        };

        buffManager.OnBuffRemoved += (buff) =>
        {
            Debug.Log($"[HealthBarController] 버프 제거 이벤트 감지: {buff.buffId} / 대상 캐릭터: {targetCharacter.name}");
            if (buff.owner == targetCharacter)
            {
                Debug.Log($"[HealthBarController] 이 캐릭터 버프 제거됨: {buff.buffId}");
                RemoveBuffIcon(buff.group);
            }
        };
    }


    public void AddBuffIcon(BuffGroup group, Sprite iconSprite)
    {
        Debug.Log($"[HealthBarController] AddBuffIcon 호출: {group}");
        if (iconSprite == null)
        {
            Debug.LogWarning($"[HealthBarController] 아이콘 스프라이트가 null임: {group}");
            return;
        }

        if (activeBuffIcons.ContainsKey(group))
        {
            var imageComponent = activeBuffIcons[group].GetComponent<Image>();
            if (imageComponent != null)
            {
                Debug.Log($"[HealthBarController] 기존 아이콘 갱신: {group}");
                imageComponent.sprite = iconSprite;
            }
            return;
        }

        Debug.Log($"[HealthBarController] 아이콘 인스턴스 생성: {group}");
        GameObject icon = Instantiate(buffIconPrefab, buffIconContainer);
        if (icon == null)
        {
            Debug.LogError("[HealthBarController] 아이콘 인스턴스화 실패");
            return;
        }

        var image = icon.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = iconSprite;
            Debug.Log($"[HealthBarController] 아이콘 스프라이트 할당 완료: {iconSprite.name}");
        }
        else
        {
            Debug.LogError("[HealthBarController] 인스턴스에 Image 컴포넌트가 없음");
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
    }
}
