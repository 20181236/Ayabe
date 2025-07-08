using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager instance { get; private set; }

    public GameObject damageTextPrefab;
    public Canvas canvas;
    public Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowDamage(Vector3 worldPosition, int damage)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition + Vector3.up * 1.5f);

        GameObject instance = Instantiate(damageTextPrefab, canvas.transform);

        // anchoredPosition 대신 직접 screenPosition 사용
        instance.GetComponent<RectTransform>().position = screenPosition;

        var controller = instance.GetComponent<DamageText>();
        controller.Setup(damage);
    }
}
