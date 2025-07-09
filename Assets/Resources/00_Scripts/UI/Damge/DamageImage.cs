using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageImage : MonoBehaviour
{
    public GameObject numberPrefab; // 숫자 하나 표시용 Image 프리팹
    public RectTransform container;
    public Sprite[] numberSprites;  // 0~9 스프라이트 배열
    public Transform numberParent;         // 자릿수 이미지들이 붙을 부모 객체
    public float spacing = 5f;      // 숫자 사이 간격

    public void ShowDamage(int damage)
    {
        //문자열...말고다른방법이있지않나
        string dmgString = damage.ToString();
        foreach (char c in dmgString)
        {
            int num = c - '0';
            GameObject digit = Instantiate(numberPrefab, transform);
            digit.GetComponent<Image>().sprite = numberSprites[num];
        }
    }

    public void ShowDamage2(int damage)
    {
        // 기존 숫자 이미지 모두 삭제
        foreach (Transform child in numberParent)
        {
            Destroy(child.gameObject);
        }

        //1234를 1,2,3,4 로
        List<int> numbers = new List<int>();
        do
        {
            numbers.Insert(0, damage % 10);
            damage /= 10;
        } while (damage > 0);

        float startX = 0f;

        for (int i = 0; i < numbers.Count; i++)
        {
            GameObject numberObject = Instantiate(numberPrefab, numberParent);
            Image image = numberObject.GetComponent<Image>();
            image.sprite = numberSprites[numbers[i]];

            RectTransform rectTransform = numberObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startX + i * (rectTransform.sizeDelta.x + spacing), 0);
        }
    }
}
