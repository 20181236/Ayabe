using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public static SkillExecutor instance { get; private set; }

    [SerializeField] public CutIn cutIn;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        if (cutIn == null)
        {
            cutIn = FindObjectOfType<CutIn>();
            if (cutIn == null)
                Debug.LogWarning("[SkillExecutor] CutIn 컴포넌트를 찾을 수 없습니다.");
        }
    }

    //public void OnSkillSelected(GameObject caster, SkillData data)
    //{

    //    if (caster == null)
    //    {
    //        Debug.LogError("SkillExecutor: caster.");
    //        return;
    //    }

    //    Debug.Log($": {data.skillId}");

    //    SkillBase skill = SkillFactory.CreateSkill(data);
    //    var context = new SkillContext
    //    {
    //        Caster = caster,
    //        Target = null,
    //        TargetPosition = Vector3.zero, // or targeting system 사용
    //        AttackPower = caster.GetComponent<PlayableBase>()?.AttackPower ?? 0f
    //    };
    //    //SkillContext context = new SkillContext
    //    //{
    //    //    Caster = caster
    //    //};

    //    //SkillEffectController.instance.EndSkillEffect();

    //    switch (data.castType)
    //    {
    //        case CastType.Instant:
    //            skill.Execute(context);
    //            break;

    //        case CastType.TargetPoint:
    //            Targeting.instance.StartPositionTargeting(data, pos =>
    //            {
    //                context.TargetPosition = pos;
    //                skill.Execute(context);
    //            });
    //            break;

    //        case CastType.TargetUnit:
    //            Targeting.instance.StartUnitTargeting(unit =>
    //            {
    //                context.Target = unit;
    //                ClearAllHighlights(); 
    //                skill.Execute(context);
    //            },
    //            unit => FilteringTeamSkill(unit, data.skillType));
    //            break;
    //    }

    //    if (data.castType == CastType.TargetUnit)
    //    {
    //        HighlightTargets(data.skillType);
    //    }
    //}
    public void OnSkillSelected(GameObject caster, SkillData data)
    {
        if (caster == null)
        {
            Debug.LogError("SkillExecutor: caster is null.");
            return;
        }

        SkillBase skill = SkillFactory.CreateSkill(data);
        var context = new SkillContext
        {
            Caster = caster,
            Target = null,
            TargetPosition = Vector3.zero,
            AttackPower = caster.GetComponent<PlayableBase>()?.AttackPower ?? 0f
        };

        switch (data.castType)
        {
            case CastType.Instant:
                InputSkill.instance.ExitSkillSelectMode();
                SkillEffectController.instance.EndSkillEffect();
                skill.Execute(context);
                ManaManager.instance.UseMana(data.manaCost);
                SkillEffectController.instance.PauseGame();
                cutIn.Play(data);
                StartCoroutine(RestoreTimeAfterDelay(1f));
                break;

            case CastType.TargetPoint:
                Targeting.instance.StartPositionTargeting(data, pos =>
                {
                    context.TargetPosition = pos;
                    InputSkill.instance.ExitSkillSelectMode();
                    SkillEffectController.instance.EndSkillEffect();
                    skill.Execute(context);
                    ManaManager.instance.UseMana(data.manaCost);
                    SkillEffectController.instance.PauseGame();
                    cutIn.Play(data);
                    StartCoroutine(RestoreTimeAfterDelay(1f));
                });
                break;

            case CastType.TargetUnit:
                Targeting.instance.StartUnitTargeting(unit =>
                {
                    context.Target = unit;
                    ClearAllHighlights();
                    InputSkill.instance.ExitSkillSelectMode();
                    SkillEffectController.instance.EndSkillEffect();
                    skill.Execute(context);
                    ManaManager.instance.UseMana(data.manaCost);
                    SkillEffectController.instance.PauseGame() ;
                    cutIn.Play(data);
                    StartCoroutine(RestoreTimeAfterDelay(2f));
                },
                unit => FilteringTeamSkill(unit, data.skillType));
                break;
        }
    }


    private bool FilteringTeamSkill(GameObject unit, SkillType skillType)
    {
        var character = unit.GetComponent<CharacterBase>();
        if (character == null)
            return false;

        switch (skillType)
        {
            case SkillType.Attack:
                return character.ObjectType == ObjectType.Enemy;
            case SkillType.Heal:
            case SkillType.Buff:
                return character.ObjectType == ObjectType.Playable;
            default:
                return false;
        }
    }

    private void HighlightTargets(SkillType skillType)
    {
        Debug.Log($"HighlightTargets called with skillType: {skillType}");

        CharacterBase[] allCharacters = FindObjectsOfType<CharacterBase>();
        foreach (var character in allCharacters)
        {
            GameObject gameoObject = character.gameObject;
            var highlight = gameoObject.GetComponent<HighlightEffect>();
            if (highlight == null)
            {
                Debug.Log($"No HighlightEffect found on {gameoObject.name}");
                continue;
            }

            bool shouldHighlight = FilteringTeamSkill(gameoObject, skillType);
            Debug.Log($"{gameoObject.name} shouldHighlight: {shouldHighlight}");
            highlight.SetHighlight(shouldHighlight);
        }
    }

    private void ClearAllHighlights()
    {
        HighlightEffect[] allHighlights = FindObjectsOfType<HighlightEffect>();
        foreach (var highlight in allHighlights)
        {
            highlight.SetHighlight(false);
        }
    }
    private IEnumerator RestoreTimeAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SkillEffectController.instance.ResumeGame();
        SkillEffectController.instance.EndSkillEffect();
    }
}