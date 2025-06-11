using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMana : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshPro currentManaText;

    private float targetFill = 1f;
    private float fillSpeed = 5f; 

    private void OnEnable()
    {
        if (ManaManager.instance != null)
            ManaManager.instance.OnManaChanged += HandleManaChanged;
    }

    private void OnDisable()
    {
        if (ManaManager.instance != null)
            ManaManager.instance.OnManaChanged -= HandleManaChanged;
    }

    private void HandleManaChanged(float ratio)
    {
        targetFill = ratio;
    }

    private void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, targetFill, Time.deltaTime * fillSpeed);
    }
}
