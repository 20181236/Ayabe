using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//시각 효과 및 타임슬로우 처리
public class SkillEffectController : MonoBehaviour
{
    public static SkillEffectController instance;

    public Image darkOverlay;
    public float slowDuration = 0.5f;
    public float slowTimeScale = 0.2f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaySkillEffect()
    {
        StartCoroutine(SkillEffectCoroutine());
    }

    private IEnumerator SkillEffectCoroutine()
    {
        // 시간 느려짐
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 화면 어두워짐
        if (darkOverlay != null)
        {
            Color color = darkOverlay.color;
            color.a = 0f;
            darkOverlay.color = color;
            darkOverlay.gameObject.SetActive(true);

            float time = 0f;
            while (time < 0.1f)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0f, 0.6f, time / 0.1f);
                darkOverlay.color = color;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(slowDuration);

        // 복원
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (darkOverlay != null)
        {
            float time = 0f;
            Color color = darkOverlay.color;
            while (time < 0.1f)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0.6f, 0f, time / 0.1f);
                darkOverlay.color = color;
                yield return null;
            }
            darkOverlay.gameObject.SetActive(false);
        }
    }
}
