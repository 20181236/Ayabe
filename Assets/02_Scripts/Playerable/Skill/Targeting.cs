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

    public GameObject positionIndicatorPrefab;
    private GameObject positionIndicatorInstance;

    // SkillData 연결
    private SkillData currentSkillData;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        mainCamera = Camera.main;

        if (positionIndicatorPrefab != null)
        {
            positionIndicatorInstance = Instantiate(positionIndicatorPrefab);
            positionIndicatorInstance.SetActive(false);
        }
    }

    //  기존 위치 요청 방식
    public void RequestPosition(Action<Vector3> callback)
    {
        onPositionSelected = callback;
        isSelectingPosition = true;
        isSelectingUnit = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(true);
    }

    //  SkillData 기반 위치 요청
    public void RequestPosition(SkillData skillData, Action<Vector3> callback)
    {
        currentSkillData = skillData;
        RequestPosition(callback); // 기본 로직 재활용
    }

    public void RequestUnit(Action<GameObject> callback, Func<GameObject, bool> filter = null)
    {
        onUnitSelected = callback;
        unitFilter = filter;
        isSelectingUnit = true;
        isSelectingPosition = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(false);

        SkillRangeVisualizer.Hide(); // 유닛 선택 시 시각화 끄기
    }

    public void UpdatePositionIndicator(Vector2 screenPosition)
    {
        if (!isSelectingPosition || positionIndicatorInstance == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            positionIndicatorInstance.transform.position = pos;

            if (currentSkillData != null)
            {
                SkillRangeVisualizer.Show(currentSkillData.skillRadius, pos);
            }
        }
        else
        {
            positionIndicatorInstance.SetActive(false);
            SkillRangeVisualizer.Hide();
        }
    }

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

        SkillRangeVisualizer.Hide(); // 시각화 숨기기
        ResetSelection();
    }

    private void ResetSelection()
    {
        isSelectingPosition = false;
        isSelectingUnit = false;
        onPositionSelected = null;
        onUnitSelected = null;
        unitFilter = null;
        currentSkillData = null;
    }

    private void Update()
    {
        if (isSelectingPosition)
        {
            UpdatePositionIndicator(Input.mousePosition);

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
