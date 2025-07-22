using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance { get; private set; }

    public List<SkillData> skillDatas = new List<SkillData>();  // 자동 등록
    public Dictionary<SkillId, SkillBase> skillInstances = new Dictionary<SkillId, SkillBase>();
    public Dictionary<PlayableID, List<SkillData>> skillsByOwner = new Dictionary<PlayableID, List<SkillData>>();

    private Dictionary<PlayableID, HashSet<SkillId>> skillAccessMap = new();

    private void InitializeSkillAccessMap()
    {
        skillAccessMap = new Dictionary<PlayableID, HashSet<SkillId>>
    {
        { PlayableID.SoonDoBu, new HashSet<SkillId> { SkillId.SoonDoBuSkill } },
        { PlayableID.Luna,     new HashSet<SkillId> { SkillId.LunaSkill } },
        { PlayableID.Ludo,     new HashSet<SkillId> { SkillId.LudoSkill } }
    };

        Debug.Log("[SkillManager] 고유 스킬 접근 맵 초기화 완료");
    }

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
        InitializeSkillAccessMap();
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
            Debug.Log($"SkillData 로드됨: {skillData.skillId}, 마나 소모: {skillData.manaCost}, 반경: {skillData.skillRadius}");
        }

        Debug.Log($"[SkillManager] 등록된 스킬 수: {skillDatas.Count}");

    }

    public void UseSkill(SkillId skillId, SkillContext context)
    {
        if (context?.Caster == null)
        {
            Debug.LogWarning("[SkillManager] Caster가 null입니다.");
            return;
        }

        PlayableBase caster = context.Caster.GetComponent<PlayableBase>();
        if (caster == null)
        {
            Debug.LogWarning("[SkillManager] Caster에 PlayableBase 컴포넌트가 없습니다.");
            return;
        }

        if (!CanUseSkill(caster.playableID, skillId))  // 이 부분 수정됨
        {
            Debug.LogWarning($"[SkillManager] {caster.playableID}는 스킬 {skillId}을 사용할 수 없습니다.");
            return;
        }

        if (skillInstances.TryGetValue(skillId, out SkillBase skill))
        {
            skill.Execute(context);
        }
        else
        {
            Debug.LogWarning($"스킬 ID {skillId} 에 해당하는 스킬 인스턴스가 없습니다.");
        }
    }

    public bool CanUseSkill(PlayableID playerId, SkillId skillId)
    {
        return skillAccessMap.TryGetValue(playerId, out var allowedSkills) &&
               allowedSkills.Contains(skillId);
    }
}
    