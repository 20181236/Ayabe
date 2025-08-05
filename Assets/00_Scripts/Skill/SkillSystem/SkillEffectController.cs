using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//시각 효과 및 타임슬로우 처리
public class SkillEffectController : MonoBehaviour
{
    public static SkillEffectController instance;

    public Image darkOverlay;
    public float fadeDuration = 0.1f;
    public float slowTimeScale = 0.2f;

    private Coroutine currentEffectCoroutine = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void StartSkillEffect()
    {
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);

        currentEffectCoroutine = StartCoroutine(SkillEffectCoroutine(true));
    }

    public void EndSkillEffect()
    {
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);

        currentEffectCoroutine = StartCoroutine(SkillEffectCoroutine(false));
    }
    public void PauseGame()
    {
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0f; // 멈춤이니 0으로 설정해도 무방합니다.
                                  // 필요하면 화면 어두워짐 등 이펙트도 여기서 실행
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        // 어두워짐 해제 등 이펙트 해제도 여기서
    }

    private IEnumerator SkillEffectCoroutine(bool enable)
    {
        float startAlpha = enable ? 0f : 0.3f;
        float endAlpha = enable ? 0.3f : 0f;
        float time = 0f;

        if (enable)
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        if (darkOverlay != null)
        {
            darkOverlay.gameObject.SetActive(true);
            Color color = darkOverlay.color;

            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
                darkOverlay.color = color;
                yield return null;
            }

            if (!enable)
                darkOverlay.gameObject.SetActive(false);
        }
    }

}