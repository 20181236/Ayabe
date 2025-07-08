using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageImage : MonoBehaviour
{
    public GameObject numberPrefab; // 숫자 하나 표시용 Image 프리팹
    public Sprite[] numberSprites;  // 0~9 스프라이트 배열

    public void ShowDamage(int damage)
    {
        string dmgStr = damage.ToString();
        foreach (char c in dmgStr)
        {
            int num = c - '0';
            GameObject digit = Instantiate(numberPrefab, transform);
            digit.GetComponent<Image>().sprite = numberSprites[num];
        }
    }
}
