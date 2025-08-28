using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

public class ToolTip : MonoBehaviour
{
    public UnityEngine.UI.Image ToolTipIcon;
    public TextMeshProUGUI ToolTipText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetToolTip(SkillData skillData)
    {
        string text = skillData.skillToolTipText;
        Sprite icon = skillData.skillToolTipIcon;
        ApplyToolTip(text, icon);
    }

    private void ApplyToolTip(string rawText, Sprite icon)
    {
        ToolTipIcon.sprite = icon;

        // 숫자를 빨간색으로 강조
        string coloredText = Regex.Replace(rawText, @"\d+", "<color=#FF5555>$0</color>");
        ToolTipText.text = coloredText;
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
