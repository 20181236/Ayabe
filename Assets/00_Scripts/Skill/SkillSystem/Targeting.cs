using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

    private List<GameObject> highlightedUnits = new List<GameObject>();

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
    private void Update()
    {
        if (isSelectingPosition)
        {
            UpdatePositionIndicator(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                ConfirmPositionSelection(Input.mousePosition);
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
    //  기존 위치 요청 방식
    public void StartPositionTargeting(Action<Vector3> callback)
    {
        onPositionSelected = callback;
        isSelectingPosition = true;
        isSelectingUnit = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(true);
    }

    //  SkillData 기반 위치 요청
    public void StartPositionTargeting(SkillData skillData, Action<Vector3> callback)
    {
        currentSkillData = skillData;
        StartPositionTargeting(callback); // 기본 로직 재활용
    }

    public void StartUnitTargeting(Action<GameObject> callback, Func<GameObject, bool> filter = null)
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
            Vector3 position = hit.point;
            positionIndicatorInstance.transform.position = position;

            if (currentSkillData != null)
            {
                float radius = currentSkillData.skillRadius;
                SkillRangeVisualizer.Show(radius, position);
                ShowHighlightInArea(position, radius); // 여기에서 radius 사용
            }
        }
        else
        {
            positionIndicatorInstance.SetActive(false);
            SkillRangeVisualizer.Hide();
            ClearHighlightedTargets();
        }
    }

    public void ConfirmPositionSelection(Vector2 screenPosition)
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

        SkillRangeVisualizer.Hide();
        ClearHighlightedTargets();
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


    private void ShowHighlightInArea(Vector3 center, float radius)
    {
        ClearHighlightedTargets();

        Collider[] hits = Physics.OverlapSphere(center, radius);
        foreach (var hit in hits)
        {
            GameObject objects = hit.gameObject;
            var highlight = objects.GetComponent<HighlightEffect>();
            if (highlight != null)
            {
                highlight.SetHighlight(true);
                highlightedUnits.Add(objects);
            }
        }
    }
    private void ClearHighlightedTargets()
    {
        foreach (var objects in highlightedUnits)
        {
            if (objects != null)
            {
                var highlight = objects.GetComponent<HighlightEffect>();
                if (highlight != null)
                    highlight.SetHighlight(false);
            }
        }

        highlightedUnits.Clear();
    }
}
