using System.Collections.Generic;

public static class ResourcePaths
{
    public const string SkillDataPath = "Datas/PlayableData/PlayableSkillData";

    public const string SkillPrefabRoot = "Prefabs/Skills/";

    private static readonly Dictionary<EffectId, string> effectPaths = new Dictionary<EffectId, string>
{
    { EffectId.SoonDoBu_CastEffect, "Art/Effect/Luna_CastEffect/ExplosionSlash" },
    { EffectId.Ludo_CastEffect, "Art/Effect/Ludo_CastEffect/DoubleSlash" },
    { EffectId.Luna_CastEffect, "Art/Effect/Luna_CastEffect/ShinySlash" }
};

    public static string GetEffectPath(EffectId effectId)
    {
        return effectPaths.TryGetValue(effectId, out string path) ? path : null;
    }

    //public const string SoonDuBuEffectPath = "Art/Effect/Luna_CastEffect/ExplosionSlash";
    //public const string LunaCastEffectPath = "Art/Effect/Luna_CastEffect/ShinySlash";
    //public const string LudoCastEffectPath = "Art/Effect/Ludo_CastEffect/DoubleSlash";
 
    public static string GetSkillPrefabPath(SkillId id)
    {
        return SkillPrefabRoot + id.ToString();
    }
    //public static string GetEffectPath(EffectId effectId)
    //{
    //    switch (effectId)
    //    {
    //        case EffectId.SoonDoBu_CastEffect:
    //            return SoonDuBuEffectPath;
    //        case EffectId.Ludo_CastEffect:
    //            return LudoCastEffectPath;
    //        case EffectId.Luna_CastEffect:
    //            return LunaCastEffectPath;
    //        default:
    //            return null;
    //    }
    //}

    private const string PopupRoot = "Plan/";

    public static string GetPopupPath(PopupList popup)
    {
        return PopupRoot + popup.ToString(); // "Plan/SetPlayablePopup"
    }


}
