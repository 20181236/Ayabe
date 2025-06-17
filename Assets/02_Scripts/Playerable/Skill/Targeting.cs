using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targeting : MonoBehaviour
{
    public static Targeting instance;

    private Action<Vector3> onPositionSelected;
    private Action<GameObject> onUnitSelected;
    private Func<GameObject, bool> unitFilter;

    private bool isSelectingPosition = false;
    private bool isSelectingUnit = false;

    private Camera mainCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        mainCamera = Camera.main;
    }

    // 위치 지정 요청
    public void RequestPosition(Action<Vector3> callback)
    {
        onPositionSelected = callback;
        isSelectingPosition = true;
        isSelectingUnit = false;

        // TODO: 위치 지정 UI 켜기
        Debug.Log("위치 지정 시작");
    }

    // 유닛 지정 요청 (필터함수는 선택 가능한 유닛 제한용)
    public void RequestUnit(Action<GameObject> callback, Func<GameObject, bool> filter = null)
    {
        onUnitSelected = callback;
        unitFilter = filter;
        isSelectingUnit = true;
        isSelectingPosition = false;

        // TODO: 유닛 선택 UI 켜기
        Debug.Log("유닛 지정 시작");
    }

    // 위치 선택 UI 업데이트 (드래그 중 위치 표시용)
    public void UpdatePositionIndicator(Vector2 screenPosition)
    {
        if (!isSelectingPosition) return;

        // 화면 좌표 -> 월드 좌표 변환
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            // TODO: 위치 지정 마커 UI 이동 처리
            Debug.Log($"위치 표시 이동: {pos}");
        }
    }

    // 위치 선택 확정 (드래그 끝났을 때 호출)
    public void ConfirmPosition(Vector2 screenPosition)
    {
        if (!isSelectingPosition) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            Debug.Log($"위치 선택 완료: {pos}");
            onPositionSelected?.Invoke(pos);
        }

        ResetSelection();
    }

    private void ResetSelection()
    {
        isSelectingPosition = false;
        isSelectingUnit = false;
        onPositionSelected = null;
        onUnitSelected = null;
        unitFilter = null;

        // TODO: UI 초기화
        Debug.Log("타겟팅 종료");
    }

    private void Update()
    {
        if (isSelectingUnit)
        {
            // 마우스 클릭으로 유닛 선택 처리 (간단 예시)
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject selected = hit.collider.gameObject;

                    // 필터가 있으면 통과해야 함
                    if (unitFilter == null || unitFilter.Invoke(selected))
                    {
                        Debug.Log($"유닛 선택 완료: {selected.name}");
                        onUnitSelected?.Invoke(selected);
                        ResetSelection();
                    }
                    else
                    {
                        Debug.Log("선택 불가능한 유닛입니다.");
                    }
                }
            }
        }
    }
}
