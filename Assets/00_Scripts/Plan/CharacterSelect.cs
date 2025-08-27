using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : UIBase
{
    public Transform selectParent;      // 캐릭터 아이콘이 들어갈 부모
    public GameObject iconPrefab;       // 아이콘 프리팹 (이미지 + 버튼)
    public Image[] selectedSlots;       // 선택된 캐릭터 슬롯 UI (최대 3개)

    private PlayableData leader;

    private List<PlayableData> selectedData = new List<PlayableData>();
    private Dictionary<PlayableData, Image> iconImages = new Dictionary<PlayableData, Image>();

    public Action<List<PlayableData>> onSelectionConfirmed;

    public void Init(List<PlayableData> allData)
    {
        foreach (Transform child in selectParent)
            Destroy(child.gameObject);

        iconImages.Clear();

        foreach (var data in allData)
        {
            GameObject icon = Instantiate(iconPrefab, selectParent);
            Image iconImage = icon.GetComponent<Image>();
            iconImage.sprite = data.playableIcon;
            iconImages[data] = iconImage;

            Button iconButton = icon.GetComponent<Button>();
            iconButton.onClick.AddListener(() => OnSelectCharacter(data));
        }

        selectedData.Clear();
        UpdateSelectedSlotsUI();
        UpdateIconAlphas();
    }

    private void OnSelectCharacter(PlayableData data)
    {
        if (selectedData.Contains(data))
            selectedData.Remove(data);
        else
        {
            if (selectedData.Count >= 3) return;
            selectedData.Add(data);
        }

        UpdateSelectedSlotsUI();
        UpdateIconAlphas();
    }

    // 선택된 캐릭터 기준으로 목록 아이콘 알파값 갱신
    private void UpdateIconAlphas()
    {
        foreach (var kvp in iconImages)
        {
            Color color = kvp.Value.color;
            color.a = selectedData.Contains(kvp.Key) ? 0.5f : 1f; // 선택된 아이콘 반투명
            kvp.Value.color = color;
        }
    }

    // 슬롯 UI 갱신 및 슬롯 클릭으로 선택 해제 가능하게 설정
    //private void UpdateSelectedSlotsUI()
    //{
    //    for (int i = 0; i < selectedSlots.Length; i++)
    //    {
    //        if (i < selectedData.Count)
    //        {
    //            selectedSlots[i].sprite = selectedData[i].playableIcon;

    //            // 버튼이 없다면 추가, 있으면 기존 이벤트 제거 후 추가
    //            Button slotButton = selectedSlots[i].GetComponent<Button>();
    //            if (slotButton == null)
    //                slotButton = selectedSlots[i].gameObject.AddComponent<Button>();
    //            slotButton.onClick.RemoveAllListeners();

    //            int index = i; // 캡처 문제 방지
    //            slotButton.onClick.AddListener(() =>
    //            {
    //                // 슬롯 클릭 시 해당 캐릭터 선택 해제
    //                selectedData.RemoveAt(index);
    //                UpdateSelectedSlotsUI();
    //            });
    //        }
    //        else
    //        {
    //            selectedSlots[i].sprite = null;

    //            // 비어있는 슬롯은 클릭 이벤트 제거
    //            Button slotButton = selectedSlots[i].GetComponent<Button>();
    //            if (slotButton != null)
    //                slotButton.onClick.RemoveAllListeners();
    //        }
    //    }
    //}
    private void UpdateSelectedSlotsUI()
    {
        for (int i = 0; i < selectedSlots.Length; i++)
        {
            if (i < selectedData.Count)
            {
                selectedSlots[i].sprite = selectedData[i].playableIcon;

                // 1번 슬롯은 항상 Leader 강조
                if (i == 0)
                    selectedSlots[i].color = Color.yellow;
                else
                    selectedSlots[i].color = Color.white;

                Button slotButton = selectedSlots[i].GetComponent<Button>();
                if (slotButton == null)
                    slotButton = selectedSlots[i].gameObject.AddComponent<Button>();
                slotButton.onClick.RemoveAllListeners();

                int index = i;
                slotButton.onClick.AddListener(() =>
                {
                    // 선택 해제
                    selectedData.RemoveAt(index);
                    // 선택 해제 후 뒤쪽 캐릭터 한 칸씩 앞으로 이동
                    UpdateSelectedSlotsUI();
                    // 아이콘 투명도 갱신
                    UpdateIconAlphas();
                });
            }
            else
            {
                selectedSlots[i].sprite = null;
                Button slotButton = selectedSlots[i].GetComponent<Button>();
                if (slotButton != null)
                    slotButton.onClick.RemoveAllListeners();
            }
        }
    }


    public void ConfirmSelection()
    {
        // 선택 완료 콜백 호출
        onSelectionConfirmed?.Invoke(selectedData);

        // 선택된 캐릭터를 싱글톤에 저장
        GameDataManager.instance.selectedCharacters = new List<PlayableData>(selectedData);

        // 1번 슬롯 캐릭터를 Leader로 지정
        GameDataManager.instance.leaderCharacter = selectedData.Count > 0 ? selectedData[0] : null;

        // Leader 확인용 로그
        if (GameDataManager.instance.leaderCharacter != null)
            Debug.Log(GameDataManager.instance.leaderCharacter.name + "가 리더입니다.");

        // 팝업 닫기
        Close();
    }
}
