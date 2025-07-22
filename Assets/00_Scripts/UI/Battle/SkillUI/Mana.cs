using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMana : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshPro currentManaText;

    private float targetFill = 1f;
    private float fillSpeed = 15f;

    private void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, targetFill, Time.deltaTime * fillSpeed);
    }
    private void OnEnable()
    {
        if (ManaManager.instance != null)
        {
            ManaManager.instance.OnManaChanged += HandleManaChanged;
            targetFill = (float)ManaManager.instance.CurrentMana / ManaManager.instance.maxMana;
            image.fillAmount = targetFill;
            currentManaText.text = ManaManager.instance.CurrentMana.ToString();
        }
    }

    private void OnDisable()
    {
        if (ManaManager.instance != null)
            ManaManager.instance.OnManaChanged -= HandleManaChanged;
    }

    private void HandleManaChanged(float ratio)
    {
        targetFill = ratio;
        if (ManaManager.instance != null)
        {
            currentManaText.text = ManaManager.instance.CurrentMana.ToString();
        }
    }


}
