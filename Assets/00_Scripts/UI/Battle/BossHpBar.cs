using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHpBar : MonoBehaviour
{
    public GameObject hpBarContainer;

    public Image nextHPBar, currentHPBar, delayHPBar;
    public TextMeshProUGUI hpText;

    public int hpSingleBar = 100;

    public int maxHP;
    public int currentHP;

    private Coroutine delayCoroutine;

    public List<Color> colors = new List<Color>();

    private void Awake()
    {
        Hide();
    }
    private void Start()
    {
        //hpBarUI.SetActive(false); // 초기에는 숨김
    }
    private void Update()
    {
        Refresh();
        //UpdateDelayBar();
    }

    public void Show()
    {
        if (hpBarContainer != null)
        {
            hpBarContainer.SetActive(true);
        }
    }

    public void Hide()
    {
        if (hpBarContainer != null)
        {
            hpBarContainer.SetActive(false); // 이 한 줄이면 자식들도 모두 꺼짐
        }
    }

    public void SetHP(int current, int max)
    {
        currentHP = current;
        maxHP = max;
        Refresh();


        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
        }
        delayCoroutine = StartCoroutine(AnimateDelayBar());
    }

    public void Refresh()
    {
        currentHPBar.rectTransform.sizeDelta = new Vector2(nextHPBar.rectTransform.sizeDelta.x * GetHPRationInSingleBar(currentHP), nextHPBar.rectTransform.sizeDelta.y);

        currentHPBar.color = GetColorByLayer(currentHP);
        nextHPBar.color = GetColorByLayer(currentHP - hpSingleBar);

        if (hpText != null)
            hpText.text = $"{currentHP} / {maxHP}";

    }

    public float GetHPRationInSingleBar(int targetHP)
    {
        float resultRaito = 0;

        if (targetHP > 0)
        {
            float divided = (float)targetHP / hpSingleBar;

            if (divided == (int)divided)
            {
                resultRaito = 1;
            }
            else
            {
                float moduled = targetHP % hpSingleBar;

                resultRaito = moduled / hpSingleBar;
            }
        }
        else
        {
            resultRaito = 0;
        }
        return resultRaito;
    }

    public Color GetColorByLayer(int targetHP)
    {
        Color result = Color.black;

        if (targetHP > 0)
        {
            float divided = (float)targetHP / hpSingleBar;

            int index = (int)divided;

            if (divided == (int)divided)
            {
                index = Mathf.Max(0, index - 1);
            }

            result = colors[index % colors.Count];
        }
        return result;
    }

    //private void UpdateDelayBar()
    //{
    //    float currentWidth = currentHPBar.rectTransform.sizeDelta.x;
    //    float delayWidth = delayHPBar.rectTransform.sizeDelta.x;

    //    if (delayWidth > currentWidth)
    //    {
    //        float distance = delayWidth - currentWidth;

    //        // 줄어드는 속도를 distance 비례로 조정 (단, 최소 200, 최대 1500 픽셀/초)
    //        float speed = Mathf.Clamp(distance * 5f, 200f, 1500f);

    //        float newWidth = Mathf.MoveTowards(delayWidth, currentWidth, speed * Time.deltaTime);
    //        delayHPBar.rectTransform.sizeDelta = new Vector2(newWidth, delayHPBar.rectTransform.sizeDelta.y);
    //    }
    //    else
    //    {
    //        delayHPBar.rectTransform.sizeDelta = new Vector2(currentWidth, delayHPBar.rectTransform.sizeDelta.y);
    //    }
    //    //    float currentWidth = currentHPBar.rectTransform.sizeDelta.x;
    //    //    float delayWidth = delayHPBar.rectTransform.sizeDelta.x;

    //    //    if (delayWidth > currentWidth)
    //    //    {
    //    //        float newWidth = Mathf.MoveTowards(delayWidth, currentWidth, delaySpeed * Time.deltaTime);
    //    //        delayHPBar.rectTransform.sizeDelta = new Vector2(newWidth, delayHPBar.rectTransform.sizeDelta.y);
    //    //    }
    //    //    else
    //    //    {
    //    //        // 만약 즉시 따라붙게 하고 싶다면 아래 줄 유지
    //    //        delayHPBar.rectTransform.sizeDelta = new Vector2(currentWidth, delayHPBar.rectTransform.sizeDelta.y);
    //    //    }
    //}
    private IEnumerator AnimateDelayBar()
    {
        float currentWidth = delayHPBar.rectTransform.sizeDelta.x;
        float targetWidth = currentHPBar.rectTransform.sizeDelta.x;

        while (currentWidth > targetWidth)
        {
            float distance = currentWidth - targetWidth;
            float speed = Mathf.Clamp(distance * 5f, 200f, 1500f);

            currentWidth = Mathf.MoveTowards(currentWidth, targetWidth, speed * Time.deltaTime);
            delayHPBar.rectTransform.sizeDelta = new Vector2(currentWidth, delayHPBar.rectTransform.sizeDelta.y);

            yield return null;
        }

        delayHPBar.rectTransform.sizeDelta = new Vector2(targetWidth, delayHPBar.rectTransform.sizeDelta.y);
        delayCoroutine = null;
    }
}
