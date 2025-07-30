using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillToolTip : MonoBehaviour
{
    public Image skillToolTipIcon;
    public TextMeshProUGUI skillToolTipText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetToolTip(SkillData skillData)
    {
        skillToolTipIcon.sprite = skillData.skillToolTipIcon;
        skillToolTipText.text = skillData.skillToolTipText;
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
