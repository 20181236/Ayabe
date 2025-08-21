using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMana : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI currentManaText;

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

        if (currentManaText != null)
        {
            int current = Mathf.RoundToInt(ratio * ManaManager.instance.maxMana);
            currentManaText.text = $"{current}";
        }
    }

    private void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, targetFill, Time.deltaTime * fillSpeed);
    }
}
