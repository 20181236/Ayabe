using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    public Image image;
    public float duration = 10.0f;

    public int maxMana=10;
    public int currentMana;

    private void Awake()
    {
        image = GetComponent<Image>();
         
    }

    private void Start()
    {
        StartCoroutine(ChangeFillAmountTime());
    }

    private IEnumerator ChangeFillAmountTime()
    {
        float currentTime = 0.0f;
        float startFillAmount = 0.0f;
        float endFillAmount = 1.0f;

        while (currentTime < duration)
        {
            float fillAmount = Mathf.Lerp(startFillAmount, endFillAmount, currentTime / duration);
            fillAmount = Mathf.Clamp01(fillAmount);

            image.fillAmount = fillAmount;
            currentTime += Time.deltaTime;

            yield return null;
        }

        image.fillAmount = endFillAmount;
    }
}
