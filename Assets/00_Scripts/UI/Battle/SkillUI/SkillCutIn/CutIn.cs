using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CutIn : MonoBehaviour
{
    public RectTransform cutInContainer;

    public Image skillImage;
    public TextMeshProUGUI skillText;

    public float slideDistance = 500f;       //밀릴 거리
    public float slideDuration = 0.4f;
    public float displayDuration = 1.5f;

    private Coroutine currentCoroutine;
    private Vector2 originalPosition;
    private Vector2 hiddenPosition = new Vector2(-1000f, -1000f);  // 화면 밖 적당한 위치 고정되어있어서 좋지않음

    private void Awake()
    {
        cutInContainer.anchoredPosition = originalPosition;
        cutInContainer.anchoredPosition = hiddenPosition;  // 처음엔 숨김
        //gameObject.SetActive(false);
    }

    public void Play(SkillData skillData)
    {
        Debug.Log($"[CutIn] Play 호출됨 for {skillData.skillId}");

        skillImage.sprite = skillData.skillCutInImage;
        skillText.text = skillData.cutInText;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        cutInContainer.anchoredPosition = hiddenPosition;
        gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(PlayRoutine());
    }
    private IEnumerator PlayRoutine()
    {
        Vector2 from = originalPosition + Vector2.left * slideDistance * 2f;
        Vector2 to = originalPosition;

        cutInContainer.anchoredPosition = from;

        // 슬라이드 인
        yield return StartCoroutine(Slide(cutInContainer, from, to, slideDuration));

        // 유지 - WaitForSeconds 대신 WaitForSecondsRealtime 사용
        yield return new WaitForSecondsRealtime(displayDuration);

        // 슬라이드 아웃
        yield return StartCoroutine(Slide(cutInContainer, to, from, slideDuration));
    }

    //private IEnumerator PlayRoutine()
    //{

    //    Vector2 from = originalPosition + Vector2.left * slideDistance * 2f;
    //    Vector2 to = originalPosition;

    //    // 시작 위치 설정
    //    cutInContainer.anchoredPosition = from;

    //    Debug.Log($"컷인 슬라이드 시작 - from: {from}, to: {to}");

    //    // 슬라이드 인
    //    yield return StartCoroutine(Slide(cutInContainer, from, to, slideDuration));

    //    // 유지
    //    yield return new WaitForSeconds(displayDuration);

    //    // 슬라이드 아웃
    //    yield return StartCoroutine(Slide(cutInContainer, to, from, slideDuration));

    //    //gameObject.SetActive(false);
    //}

    //private IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    //{
    //    float time = 0f;
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float t = Mathf.Clamp01(time / duration);
    //        target.anchoredPosition = Vector2.Lerp(from, to, t);
    //        yield return null;
    //    }

    //    target.anchoredPosition = to;
    //}
    private IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;  // 변경: Time.deltaTime -> Time.unscaledDeltaTime
            float t = Mathf.Clamp01(time / duration);
            target.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        target.anchoredPosition = to;
    }
}
