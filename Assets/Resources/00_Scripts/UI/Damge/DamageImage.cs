using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System;

    public class DamageImage : MonoBehaviour
    {
    [Header("Required References")]
    public RectTransform canvasRect;
    public GameObject numberPrefab;
    public Sprite[] numberSprites;

    [Header("Settings")]
    public float spacing = 10f;
    public float floatUpDistance = 30f;
    public float duration = 1f;

    private HorizontalLayoutGroup layout;

    private void Awake()
    {
        LoadNumberSpritesFromAtlas();

        // HorizontalLayoutGroup을 미리 붙여두고 세팅
        layout = GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
        {
            layout = gameObject.AddComponent<HorizontalLayoutGroup>();
        }
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
    }

    private void LoadNumberSpritesFromAtlas()
    {
        numberSprites = new Sprite[10];

        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIResources/UIZop/DamgeSprites/DamageAtlas2");

        if (atlas == null)
        {
            Debug.LogError("SpriteAtlas를 불러올 수 없습니다. 경로를 확인하세요.");
            return;
        }

        for (int i = 0; i <= 9; i++)
        {
            string spriteName = $"numbers_{i}";
            Sprite sprite = atlas.GetSprite(spriteName);

            numberSprites[i] = sprite;
        }
    }

    public void ShowDamageImage(int damage, Vector3 worldPosition)
    {
        if (damage == 0)
            return;

        // 기존 자식 전부 제거 (초기화)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // damage 각 자리 숫자를 나눠서 이미지 생성
        List<int> digits = new List<int>();
        while (damage > 0)
        {
            digits.Insert(0, damage % 10);
            damage /= 10;
        }

        foreach (int digit in digits)
        {
            GameObject numbersObject = Instantiate(numberPrefab, transform);
            Image img = numbersObject.GetComponent<Image>();
            img.sprite = numberSprites[digit];
        }

        StartCoroutine(RiseAndFade(gameObject));
    }

    private IEnumerator RiseAndFade(GameObject gameObject)
    {
        float elapsed = 0f;
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = startPosition + Vector2.up * (floatUpDistance * t);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        Destroy(gameObject);
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.U2D;
//using UnityEngine.UI;

//public class DamageImage : MonoBehaviour
//{
//    [Header("Required References")]
//    public RectTransform canvasRect;     // 데미지 UI용 Canvas (Screen Space)
//    public GameObject numberPrefab;      // 숫자 하나당 Image 프리팹
//    public Sprite[] numberSprites;       // Atlas에 포함된 0~9 스프라이트
//    public SpriteAtlas numberAtlas; 

//    [Header("Settings")]
//    public float spacing = 10f;          // 숫자 간격
//    public float floatUpDistance = 30f;  // 떠오르는 거리
//    public float duration = 1f;          // 애니메이션 지속 시간

//    private void Awake()
//    {
//        LoadNumberSpritesFromAtlas();
//    }

//    private void LoadNumberSpritesFromAtlas()
//    {
//        numberSprites = new Sprite[10];
//        for (int i = 0; i <= 9; i++)
//        {
//            string spriteName = "numbers_" + i;
//            numberSprites[i] = numberAtlas.GetSprite(spriteName);
//            if (numberSprites[i] == null)
//                Debug.LogWarning($"SpriteAtlas에서 '{spriteName}' 스프라이트를 못 찾았습니다.");
//        }
//    }

//    public void ShowDamage(int damage, Vector3 worldPosition)
//    {
//        Debug.Log($"[DEBUG] ShowDamage called: {damage}, prefab: {numberPrefab}, atlas: {numberAtlas}");

//        foreach (var sprite in numberSprites)
//        {
//            Debug.Log(sprite != null ? $"Sprite OK: {sprite.name}" : "Sprite MISSING");
//        }

//        if (damage == 0)
//            return; // 0은 표시하지 않음

//        // 월드 → 스크린 좌표 변환
//        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

//        // 데미지 숫자 UI 오브젝트 생성
//        GameObject damageGameObject = new GameObject("DamageUI", typeof(RectTransform));
//        damageGameObject.transform.SetParent(canvasRect, false);

//        RectTransform rectTransform = damageGameObject.GetComponent<RectTransform>();
//        rectTransform.anchoredPosition = screenPosition;

//        // 숫자 가로 정렬
//        HorizontalLayoutGroup layout = damageGameObject.AddComponent<HorizontalLayoutGroup>();
//        layout.spacing = spacing;
//        layout.childAlignment = TextAnchor.MiddleCenter;

//        // 숫자 자릿수 분해 (0 제외)
//        List<int> digits = new List<int>();
//        while (damage > 0)
//        {
//            digits.Insert(0, damage % 10);
//            damage /= 10;
//        }

//        // 숫자 프리팹 생성 및 스프라이트 지정
//        foreach (int digit in digits)
//        {
//            GameObject digitObj = Instantiate(numberPrefab, damageGameObject.transform);
//            Image img = digitObj.GetComponent<Image>();
//            img.sprite = numberSprites[digit];
//        }

//        // 떠오르고 사라지는 애니메이션
//        StartCoroutine(RiseAndFade(damageGameObject));
//    }

//    private IEnumerator RiseAndFade(GameObject gameObject)
//    {
//        float elapsed = 0f;
//        CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
//        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
//        Vector2 startPosition = rectTransform.anchoredPosition;

//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            float t = elapsed / duration;

//            // 위로 이동
//            rectTransform.anchoredPosition = startPosition + Vector2.up * (floatUpDistance * t);
//            // 점점 투명하게
//            canvasGroup.alpha = 1f - t;

//            yield return null;
//        }

//        Destroy(gameObject);
//    }
//}
