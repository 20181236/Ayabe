using System;
using System.Collections.Generic;
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

    [SerializeField] private LayerMask characterLayer;

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

    // 위치 선택 요청 (기본)
    public void RequestPosition(Action<Vector3> callback)
    {
        onPositionSelected = callback;
        isSelectingPosition = true;
        isSelectingUnit = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(true);
    }

    // SkillData 기반 위치 선택 요청
    public void RequestPosition(SkillData skillData, Action<Vector3> callback)
    {
        currentSkillData = skillData;
        RequestPosition(callback);
    }

    // 유닛 선택 요청
    public void RequestUnit(Action<GameObject> callback, Func<GameObject, bool> filter = null)
    {
        onUnitSelected = callback;
        unitFilter = filter;
        isSelectingUnit = true;
        isSelectingPosition = false;

        if (positionIndicatorInstance != null)
            positionIndicatorInstance.SetActive(false);

        SkillRangeVisualizer.Hide();
    }

    private void UpdatePositionIndicator(Vector2 screenPosition)
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

                // Physics OverlapSphere 기반 하이라이트
                HighlightUnitsInRadius(pos, currentSkillData);
            }
        }
        else
        {
            positionIndicatorInstance.SetActive(false);
            SkillRangeVisualizer.Hide();
            ClearAllHighlights();
        }
    }

    private void HighlightUnitsInRadius(Vector3 center, SkillData data)
    {
        ClearAllHighlights();

        Collider[] hitColliders = Physics.OverlapSphere(center, data.skillRadius, characterLayer);

        foreach (var collider in hitColliders)
        {
            GameObject obj = collider.gameObject;
            var highlight = obj.GetComponent<HighlightEffect>();
            if (highlight == null)
                continue;

            bool isValid = SkillExecutor.instance.FilteringTeamSkill(obj, data.skillType);
            if (isValid)
            {
                highlight.SetHighlight(true);
            }
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

        SkillRangeVisualizer.Hide();
        ClearAllHighlights();
        ResetSelection();
    }

    private void ClearAllHighlights()
    {
        HighlightEffect[] allHighlights = FindObjectsOfType<HighlightEffect>();
        foreach (var highlight in allHighlights)
        {
            highlight.SetHighlight(false);
        }
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
