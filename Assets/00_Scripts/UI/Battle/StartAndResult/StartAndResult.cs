using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartAndResult : MonoBehaviour
{
    [SerializeField] private GameObject darkOverlay;

    [SerializeField] private GameObject startImage;

    [SerializeField] private GameObject victoryImage;
    [SerializeField] private GameObject victoryPanel;

    [SerializeField] private GameObject defeatImage;
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private TextMeshPro battleResultText;
    [SerializeField] private TextMeshPro battleTimeText;

    private void Awake()
    {
        // UI 전부 꺼둠
        darkOverlay.SetActive(false);
        startImage.SetActive(false);
        victoryImage.SetActive(false);
        defeatImage.SetActive(false);
    }

    public void ShowStartSequence(float startDuration)
    {
        StartCoroutine(PlayStartSequence(startDuration));
    }

    public IEnumerator PlayStartSequence(float duration)
    {
        darkOverlay.SetActive(true);
        startImage.SetActive(true);

        yield return new WaitForSeconds(duration);

        startImage.SetActive(false);
        darkOverlay.SetActive(false);
    }

    public void SetElapsedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        battleTimeText.text = $"전투 시간: {minutes:00}:{seconds:00}";
    }


    public void ShowVictory()
    {
        darkOverlay.SetActive(true);    
        victoryImage.SetActive(true);
    }

    public void ShowDefeat()
    {
        darkOverlay.SetActive(true);
        defeatImage.SetActive(true);
    }

    // 결과 UI 출력용 함수
    public void ShowResultUI(bool isVictory, float battleTime)
    {
        string formattedTime = FormatTime(battleTime);
        battleTimeText.text = $"소요 시간: {formattedTime}";

        if (isVictory)
        {
            battleResultText.text = "이겼다!";
            victoryPanel.SetActive(true);
        }
        else
        {
            battleResultText.text = "졌다...";
            defeatPanel.SetActive(true);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes}분 {seconds}초";
    }
}
