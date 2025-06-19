using System;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public static Targeting instance;

    private Action<Vector3> onPositionSelected;
    private Action<GameObject> onUnitSelected;
    private Func<GameObject, bool> unitFilter;

    private bool isSelectingPosition = false;
    private bool isSelectingUnit = false;

    private Camera mainCamera;

    [Tooltip("위치 표시용 프리팹 (예: 투명한 원형 오브젝트)")]
    public GameObject positionIndicatorPrefab;
    private GameObject positionIndicatorInstance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        mainCamera = Camera.main;

        // 위치 표시 오브젝트 생성 및 비활성화
        if (positionIndicatorPrefab != null)
        {
            positionIndicatorInstance = Instantiate(positionIndicatorPrefab);
            positionIndicatorInstance.SetActive(false);
        }
    }

    public void RequestPosition(Action<Vector3> callback)
    {
        onPositionSelected = callback;
        isSelectingPosition = true;
        isSelectingUnit = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(true);
    }

    public void RequestUnit(Action<GameObject> callback, Func<GameObject, bool> filter = null)
    {
        onUnitSelected = callback;
        unitFilter = filter;
        isSelectingUnit = true;
        isSelectingPosition = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(false);
    }

    // 마우스 위치에 따라 위치 표시 오브젝트 이동
    public void UpdatePositionIndicator(Vector2 screenPosition)
    {
        if (!isSelectingPosition || positionIndicatorInstance == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            positionIndicatorInstance.transform.position = pos;
        }
        else
        {
            positionIndicatorInstance.SetActive(false);
        }
    }

    // 위치 확정, 콜백 호출 및 상태 초기화
    public void ConfirmPosition(Vector2 screenPosition)
    {
        if (!isSelectingPosition)
            return;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 position = hit.point;
            onPositionSelected?.Invoke(position);
        }

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(false);

        ResetSelection();
    }

    private void ResetSelection()
    {
        isSelectingPosition = false;
        isSelectingUnit = false;
        onPositionSelected = null;
        onUnitSelected = null;
        unitFilter = null;
    }

    private void Update()
    {
        if (isSelectingPosition)
        {
            // 매 프레임 위치 표시 업데이트
            UpdatePositionIndicator(Input.mousePosition);

            // 마우스 클릭 시 위치 확정
            if (Input.GetMouseButtonDown(0))
            {
                ConfirmPosition(Input.mousePosition);
            }
        }
        else if (isSelectingUnit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject selected = hit.collider.gameObject;
                    if (unitFilter == null || unitFilter.Invoke(selected))
                    {
                        onUnitSelected?.Invoke(selected);
                        ResetSelection();
                    }
                }
            }
        }
    }
}
