using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

public class SkillToolTip : MonoBehaviour
{
    public UnityEngine.UI.Image skillToolTipIcon;
    public TextMeshProUGUI skillToolTipText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetToolTip(SkillData skillData)
    {
        skillToolTipIcon.sprite = skillData.skillToolTipIcon;
        skillToolTipText.text = skillData.skillToolTipText;

        string text = skillToolTipText.text;
        // 숫자를 빨간색으로 강조
        text = Regex.Replace(text, @"\d+", "<color=#FF5555>$0</color>");

        skillToolTipText.text = text;
    }

    public void Show(SkillData skillData)
    {
        SetToolTip(skillData);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
