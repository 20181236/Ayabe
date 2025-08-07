using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenAndTimeEffectController : MonoBehaviour
{
    public static ScreenAndTimeEffectController instance;

    public Image darkOverlay;
    public float fadeDuration = 0.1f;
    public float slowTimeScale = 0.2f;

    private Coroutine currentEffectCoroutine = null;
    private Coroutine fadeCoroutine = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    //기존 호출용 - 시간 슬로우 + 페이드인
    public void StartEffect()
    {
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);

        currentEffectCoroutine = StartCoroutine(CombinedEffectCoroutine(true));
    }

    //기존 호출용 - 시간 정상화 + 페이드아웃
    public void EndEffect()
    {
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);

        currentEffectCoroutine = StartCoroutine(CombinedEffectCoroutine(false));
    }

    //시간 멈춤용
    public void PauseGame()
    {
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    //시간만 느리게
    public void SlowTime()
    {
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * slowTimeScale;
    }

    //시간만 원래대로
    public void RestoreTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    //외부에서 직접 호출 가능한 시각 효과(Fade)
    public void FadeInOverlay()
    {
        StartFadeOverlay(0f, 0.3f);
    }

    public void FadeOutOverlay()
    {
        StartFadeOverlay(0.3f, 0f);
    }

    private void StartFadeOverlay(float fromAlpha, float toAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOverlayCoroutine(fromAlpha, toAlpha));
    }

    private IEnumerator FadeOverlayCoroutine(float startAlpha, float endAlpha)
    {
        if (darkOverlay == null)
            yield break;

        darkOverlay.gameObject.SetActive(true);
        Color color = darkOverlay.color;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            darkOverlay.color = color;
            yield return null;
        }

        color.a = endAlpha;
        darkOverlay.color = color;

        if (endAlpha <= 0f)
            darkOverlay.gameObject.SetActive(false);
    }

    //기존 기능 유지(시간 + 페이드 동시에)
    private IEnumerator CombinedEffectCoroutine(bool enable)
    {
        if (enable)
        {
            SlowTime();
            FadeInOverlay();
        }
        else
        {
            RestoreTime();
            FadeOutOverlay();
        }

        yield break; // 동시실행 구조 유지
    }
}

//시각 효과 및 타임슬로우 처리
//public class SkillEffectController : MonoBehaviour
//{
//    public static SkillEffectController instance;

//    public Image darkOverlay;
//    public float fadeDuration = 0.1f;
//    public float slowTimeScale = 0.2f;

//    private Coroutine currentEffectCoroutine = null;

//    private void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }

//    public void StartSkillEffect()
//    {
//        if (currentEffectCoroutine != null)
//            StopCoroutine(currentEffectCoroutine);

//        currentEffectCoroutine = StartCoroutine(SkillEffectCoroutine(true));
//    }

//    public void EndSkillEffect()
//    {
//        if (currentEffectCoroutine != null)
//            StopCoroutine(currentEffectCoroutine);

//        currentEffectCoroutine = StartCoroutine(SkillEffectCoroutine(false));
//    }
//    public void PauseGame()
//    {
//        Time.timeScale = 0.01f;
//        Time.fixedDeltaTime = 0f; // 멈춤이니 0으로 설정해도 무방합니다.
//                                  // 필요하면 화면 어두워짐 등 이펙트도 여기서 실행
//    }
//    public void ResumeGame()
//    {
//        Time.timeScale = 1f;
//        Time.fixedDeltaTime = 0.02f;
//        // 어두워짐 해제 등 이펙트 해제도 여기서
//    }

//    private IEnumerator SkillEffectCoroutine(bool enable)
//    {
//        float startAlpha = enable ? 0f : 0.3f;
//        float endAlpha = enable ? 0.3f : 0f;
//        float time = 0f;

//        if (enable)
//        {
//            Time.timeScale = slowTimeScale;
//            Time.fixedDeltaTime = 0.02f * Time.timeScale;
//        }
//        else
//        {
//            Time.timeScale = 1f;
//            Time.fixedDeltaTime = 0.02f;
//        }

//        if (darkOverlay != null)
//        {
//            darkOverlay.gameObject.SetActive(true);
//            Color color = darkOverlay.color;

//            while (time < fadeDuration)
//            {
//                time += Time.unscaledDeltaTime;
//                color.a = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
//                darkOverlay.color = color;
//                yield return null;
//            }

//            if (!enable)
//                darkOverlay.gameObject.SetActive(false);
//        }
//    }

//}