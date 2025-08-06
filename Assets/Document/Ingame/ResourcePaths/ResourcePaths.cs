public static class ResourcePaths
{
    public const string SkillDataPath = "Datas/PlayableData/PlayableSkillData";

    public const string SkillPrefabRoot = "Prefabs/Skills/";

    public const string LudoCastEffectPath = "Art/Effect/Ludo_CastEffect/DoubleSlash";
    public const string LunaCastEffectPath = "Art/Effect/Luna_CastEffect";


    public static string GetSkillPrefabPath(SkillId id)
    {
        return SkillPrefabRoot + id.ToString(); // ¿¹: "Prefabs/Skills/LudoSkill"
    }
    public static string GetEffectPath(EffectId effectId)
    {
        switch (effectId)
        {
            case EffectId.Ludo_CastEffect:
                return LudoCastEffectPath;
            case EffectId.Luna_CastEffect:
                return LunaCastEffectPath;
            default:
                return null;
        }
    }
}
