using UnityEngine;

public class SkillRangeIndicator : MonoBehaviour
{
    public GameObject skillRangeIndicatorInstance;
    public SkillData skillData;

    void Start()
    {
        skillRangeIndicatorInstance.SetActive(false);
    }

    public void SetSkill(SkillData newSkillData)
    {
        skillData = newSkillData;
    }

    public void ShowSkillRange()
    {
        if (skillData == null)
        {
            Debug.LogWarning("SkillData is null!");
            return;
        }

        skillRangeIndicatorInstance.SetActive(true);
        skillRangeIndicatorInstance.transform.position = transform.position;

        float radius = skillData.skillRadius;
        skillRangeIndicatorInstance.transform.localScale = new Vector3(radius, radius, radius);
    }

    public void HideSkillRange()
    {
        skillRangeIndicatorInstance.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ShowSkillRange();

        if (Input.GetMouseButtonUp(1))
            HideSkillRange();
    }
}
