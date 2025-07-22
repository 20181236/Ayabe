using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHpBar : MonoBehaviour
{
    public GameObject hpBarUI;

    private void Start()
    {
        hpBarUI.SetActive(false);
    }
    private void Update()
    {
        
    }

    public void Show()
    {
        hpBarUI.SetActive(true);  // 보스 등장 시 보여줌
    }

    public void Hide()
    {
        hpBarUI.SetActive(false); // 보스죽고 게임 끝날때
    }

}
