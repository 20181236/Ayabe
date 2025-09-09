using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartAndResult : MonoBehaviour
{
    [Header("Common UI")]
    [SerializeField] private GameObject darkOverlay;

    [Header("Start UI")]
    [SerializeField] private GameObject startImage;

    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite victorySprite;
    [SerializeField] private Sprite defeatSprite;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI battleTimeText;

    [SerializeField] private Button okButton;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private GameObject uiHUD;

    [SerializeField] private GameObject skillUI;



    private void Awake()
    {
        // 모든 UI 초기 비활성화
        darkOverlay.SetActive(false);
        startImage.SetActive(false);
        resultPanel.SetActive(false);
        okButton.gameObject.SetActive(false);
        //skillUI.gameObject.SetActive(false);
    }

    public IEnumerator PlayStartSequence(float duration)
    {
        darkOverlay.SetActive(true);
        startImage.SetActive(true);

        yield return new WaitForSeconds(duration);

        startImage.SetActive(false);
        darkOverlay.SetActive(false);

        bossHealthBar.SetActive(true);
        skillUI.gameObject.SetActive(true);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public void ShowUI(StageState state, float battleTime = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowUISequence(state, battleTime));
    }

    private IEnumerator ShowUISequence(StageState state, float battleTime)
    {
        uiHUD.SetActive(false);
        healthBar.SetActive(false);
        //bossHealthBar.SetActive(false);

        // 공통: 배경
        darkOverlay.SetActive(state != StageState.None);

        // 시작 상태면 그냥 시작 이미지만 보여주고 끝
        if (state == StageState.Starting)
        {
            startImage.SetActive(true);
            yield break;
        }

        // 승리/패배 시
        if (state == StageState.Victory || state == StageState.Defeat)
        {
            // 패널 켜기
            resultPanel.SetActive(true);

            // 결과 이미지 설정
            resultImage.sprite = (state == StageState.Victory) ? victorySprite : defeatSprite;
            resultText.text = (state == StageState.Victory) ? "Win!" : "Defeat...";
            battleTimeText.text = $"소요 시간: {FormatTime(battleTime)}";

            // 결과 이미지 보여주기
            resultImage.gameObject.SetActive(true);

            // 몇 초 대기
            yield return new WaitForSeconds(3f);

            // 결과 이미지 숨기기
            resultImage.gameObject.SetActive(false);

            // 여기서 나머지 UI(버튼, 상세 결과 패널 등) 켜기
            okButton.gameObject.SetActive(true);
        }
    }
    public void ShowStageStartUI(bool hasBoss)
    {
        uiHUD.SetActive(true);
        healthBar.SetActive(true);

        // 전달받은 hasBoss 정보를 사용 (더 이상 StageManager를 참조하지 않음)
        bossHealthBar.SetActive(hasBoss);
    }
}
