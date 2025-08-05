public static class ResourcePaths
{
    public const string SkillDataPath = "Datas/PlayableData/PlayableSkillData";

    public const string SkillPrefabRoot = "Prefabs/Skills/";

    public const string LudoCastEffect = "Art/Effect/Ludo_CastEffect";


    public static string GetSkillPrefabPath(SkillId id)
    {
        return SkillPrefabRoot + id.ToString(); // ¿¹: "Prefabs/Skills/LudoSkill"
    }
}

