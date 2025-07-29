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

    private void Awake()
    {
        cutInContainer.anchoredPosition = originalPosition;
        gameObject.SetActive(false);
    }

    public void Play(SkillData skillData)
    {
        Debug.Log("[CutIn] Play 호출됨");

        skillImage.sprite = skillData.skillCutInImage;
        skillText.text = skillData.cutInText;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {

        Vector2 from = originalPosition + Vector2.left * slideDistance * 2f;
        Vector2 to = originalPosition;

        // 시작 위치 설정
        cutInContainer.anchoredPosition = from;

        Debug.Log($"컷인 슬라이드 시작 - from: {from}, to: {to}");

        // 슬라이드 인
        yield return StartCoroutine(Slide(cutInContainer, from, to, slideDuration));

        // 유지
        yield return new WaitForSeconds(displayDuration);

        // 슬라이드 아웃
        yield return StartCoroutine(Slide(cutInContainer, to, from, slideDuration));

        gameObject.SetActive(false);
    }

    private IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            target.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        target.anchoredPosition = to;
    }
}
