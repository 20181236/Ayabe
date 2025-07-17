using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance { get; private set; }

    public List<SkillData> skillDatas = new List<SkillData>();  // 자동 등록
    public Dictionary<SkillId, SkillBase> skillInstances = new Dictionary<SkillId, SkillBase>();
    public Dictionary<PlayableID, List<SkillData>> skillsByOwner = new Dictionary<PlayableID, List<SkillData>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadSkillsFromResources();
    }

    private void LoadSkillsFromResources()
    {
        SkillData[] loaded = Resources.LoadAll<SkillData>(ResourcePaths.SkillDataPath);
        skillDatas = new List<SkillData>(loaded);

        foreach (var skillData in skillDatas)
        {
            if (!skillsByOwner.ContainsKey(skillData.ownerId))
                skillsByOwner[skillData.ownerId] = new List<SkillData>();

            skillsByOwner[skillData.ownerId].Add(skillData);

            var skill = SkillFactory.CreateSkill(skillData);
            if (skill != null)
                skillInstances[skillData.skillId] = skill;
        }

        Debug.Log($"[SkillManager] 등록된 스킬 수: {skillDatas.Count}");
    }

    public void UseSkill(SkillId skillId, SkillContext context)
    {
        if (skillInstances.TryGetValue(skillId, out SkillBase skill))
        {
            skill.Execute(context);
        }
        else
        {
            Debug.LogWarning($"스킬 ID {skillId} 에 해당하는 스킬 인스턴스가 없습니다.");
        }
    }
}
