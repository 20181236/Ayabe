using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System;

public class DamageImage : MonoBehaviour
{
    [Header("Required References")]
    public GameObject numberPrefab;
    public Sprite[] numberSprites;

    [Header("Settings")]
    public float spacing = 10f;
    public float floatUpDistance = 50f; // 월드 공간 기준 상승 거리 (예: 1미터)
    public float duration = 1f;

    private HorizontalLayoutGroup layout;

    private void Awake()
    {
        LoadNumberSpritesFromAtlas();

        // HorizontalLayoutGroup을 가져오거나 없다면 추가하여 설정합니다.
        layout = GetComponent<HorizontalLayoutGroup>() ?? gameObject.AddComponent<HorizontalLayoutGroup>();
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

    /// <summary>
    /// 지정된 월드 위치에 데미지 숫자를 표시합니다.
    /// </summary>
    /// <param name="damage">표시할 데미지 값</param>
    /// <param name="worldPosition">숫자가 생성될 3D 월드 좌표</param>
    public void ShowDamageImage(int damage, Vector3 worldPosition)
    {
        if (damage == 0)
        {
            Destroy(gameObject); // 데미지가 0이면 바로 파괴
            return;
        }
        float randomRadius = 3f; // 랜덤 범위 (이 값을 조절해 흩어지는 정도를 바꿀 수 있습니다)
        Vector3 randomOffset = Camera.main.transform.right * UnityEngine.Random.Range(-randomRadius, randomRadius);

        // 기존 위치 계산에 랜덤 오프셋을 더해줍니다.
        // 기존 코드: transform.position = worldPosition + Vector3.up * 3f;
        transform.position = worldPosition + Vector3.up * 3f + randomOffset; 

        // 이전에 생성된 숫자 이미지가 있다면 모두 제거합니다.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 데미지 값을 각 자리수로 분해합니다.
        string damageString = damage.ToString();
        foreach (char digitChar in damageString)
        {
            int digit = digitChar - '0'; // char를 int로 변환

            GameObject numberObject = Instantiate(numberPrefab, transform);
            Image img = numberObject.GetComponent<Image>();
            img.sprite = numberSprites[digit];
        }

        StartCoroutine(RiseAndFade());
    }

    /// <summary>
    /// 오브젝트를 위로 올리면서 서서히 투명하게 만든 후 파괴하는 코루틴입니다.
    /// </summary>
    private IEnumerator RiseAndFade()
    {
        float elapsed = 0f;
        // CanvasGroup이 없다면 추가합니다.
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        // 월드 공간에서의 애니메이션을 위해 Local Position을 사용합니다.
        Vector3 startPosition = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 월드 공간에서 Vector3.up 방향으로 상승시킵니다.
            transform.localPosition = startPosition + Vector3.up * (floatUpDistance * t);

            // CanvasGroup의 alpha 값을 조절하여 투명하게 만듭니다.
            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        // 애니메이션이 끝나면 오브젝트를 파괴합니다.
        Destroy(gameObject);
    }
}

//    public class DamageImage : MonoBehaviour
//    {
//    [Header("Required References")]
//    //public RectTransform canvasRect;
//    public GameObject numberPrefab;
//    public Sprite[] numberSprites;

//    [Header("Settings")]
//    public float spacing = 10f;
//    public float floatUpDistance = 1f;//30f;
//    public float duration = 1f;

//    private HorizontalLayoutGroup layout;

//    private void Awake()
//    {
//        LoadNumberSpritesFromAtlas();

//        // HorizontalLayoutGroup을 미리 붙여두고 세팅
//        layout = GetComponent<HorizontalLayoutGroup>();
//        if (layout == null)
//        {
//            layout = gameObject.AddComponent<HorizontalLayoutGroup>();
//        }
//        layout.spacing = spacing;
//        layout.childAlignment = TextAnchor.MiddleCenter;
//    }

//    private void LoadNumberSpritesFromAtlas()
//    {
//        numberSprites = new Sprite[10];

//        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIResources/UIZop/DamgeSprites/DamageAtlas2");

//        if (atlas == null)
//        {
//            Debug.LogError("SpriteAtlas를 불러올 수 없습니다. 경로를 확인하세요.");
//            return;
//        }

//        for (int i = 0; i <= 9; i++)
//        {
//            string spriteName = $"numbers_{i}";
//            Sprite sprite = atlas.GetSprite(spriteName);

//            numberSprites[i] = sprite;
//        }
//    }

//    public void ShowDamageImage(int damage, Vector3 worldPosition)
//    {
//        if (damage == 0)
//            return;

//        // 변환된 스크린 좌표로 자신의 위치를 설정합니다.
//        transform.position = worldPosition;

//        // 기존 자식 전부 제거 (초기화)
//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }

//        // damage 각 자리 숫자를 나눠서 이미지 생성
//        List<int> digits = new List<int>();
//        while (damage > 0)
//        {
//            digits.Insert(0, damage % 10);
//            damage /= 10;
//        }

//        foreach (int digit in digits)
//        {
//            GameObject numbersObject = Instantiate(numberPrefab, transform);
//            Image img = numbersObject.GetComponent<Image>();
//            img.sprite = numberSprites[digit];
//        }

//        //StartCoroutine(RiseAndFade(gameObject));
//        StartCoroutine(RiseAndFade());
//    }
//    private IEnumerator RiseAndFade()
//    {
//        float elapsed = 0f;
//        CanvasGroup canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

//        Vector3 startPosition = transform.localPosition;

//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            float t = elapsed / duration;

//            // 변경: Vector2.up -> Vector3.up, 월드 공간에서 위로 이동
//            transform.localPosition = startPosition + Vector3.up * (floatUpDistance * t);

//            canvasGroup.alpha = 1f - t;
//            yield return null;
//        }

//        Destroy(gameObject);
//    }
//    //private IEnumerator RiseAndFade(GameObject gameObject)
//    //{
//    //    float elapsed = 0f;
//    //    CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
//    //    if (canvasGroup == null)
//    //        canvasGroup = gameObject.AddComponent<CanvasGroup>();

//    //    RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
//    //    Vector2 startPosition = rectTransform.anchoredPosition;

//    //    while (elapsed < duration)
//    //    {
//    //        elapsed += Time.deltaTime;
//    //        float t = elapsed / duration;
//    //        rectTransform.anchoredPosition = startPosition + Vector2.up * (floatUpDistance * t);
//    //        canvasGroup.alpha = 1f - t;
//    //        yield return null;
//    //    }

//    //    Destroy(gameObject);
//    //}
//}
