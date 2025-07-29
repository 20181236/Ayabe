using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHpBar : MonoBehaviour
{
    public TextMeshProUGUI hpText;

    //public GameObject hpBarUI;
    public Image nextHPBar, currentHPBar;

    public int hpSingleBar = 20;

    public int maxHP;
    public int currentHP;

    public List<Color> colors = new List<Color>();

    private void Start()
    {
        //hpBarUI.SetActive(false); // 초기에는 숨김
    }
    private void Update()
    {
        Refresh();
    }

    public void Show()
    {
        //hpBarUI.SetActive(true);  // 보스 등장 시 보여줌
    }

    public void Hide()
    {
        //hpBarUI.SetActive(false); // 보스죽고 게임 끝날때
    }
    public void SetHP(int current, int max)
    {
        currentHP = current;
        maxHP = max;
        Refresh();
    }

    public void Refresh()
    {
        currentHPBar.rectTransform.sizeDelta = new Vector2(nextHPBar.rectTransform.sizeDelta.x * GetHPRationInSingleBar(currentHP), nextHPBar.rectTransform.sizeDelta.y);

        currentHPBar.color = GetColorByLayer(currentHP);
        nextHPBar.color = GetColorByLayer(currentHP - hpSingleBar);

        if (hpText != null)
            hpText.text = $"{currentHP} / {maxHP}";

        Debug.Log($"Base Width: {nextHPBar.rectTransform.sizeDelta.x}, Ratio: {GetHPRationInSingleBar(currentHP)}, Result Width: {currentHPBar.rectTransform.sizeDelta.x}");
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
}
