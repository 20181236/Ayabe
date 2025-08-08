using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Transform target;  // 체력바를 따라다닐 캐릭터 머리 위치
    [SerializeField] private Camera mainCamera; // 메인 카메라
    [SerializeField] private Image fillImage;   // 체력바 이미지 (Fill Amount 조절용)

    private float maxHealth;
    private float currentHealth;


    private RectTransform rectTransform;

    [SerializeField] private Vector3 offset = new Vector3(0, 5f, 0);    

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
}
