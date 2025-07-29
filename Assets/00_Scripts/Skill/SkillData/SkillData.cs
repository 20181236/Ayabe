using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillId skillId;
    public PlayableID ownerId; // 이 스킬의 주인
    public SkillType skillType;
    public CastType castType;
    public AreaType areaType;   // 장판이 딜인지 힐인지 구분

    public GameObject weaponPrefab;
    public Sprite icon;

    public Sprite skillCutInImage;
    public string cutInText;

    public string skillTooltipText;

    public GameObject caster;

    public int manaCost;

    public float range;
    public float skillRadius;
    public float skillAngle;

    public float damageMultiplier;
    public float healValue; //즉시 힐, 지속 힐 총량

    public float duration;
    public float tickInterval; 

    //장판
    public float effectAmount;        // 딜 혹은 힐량 (healValue 대신 통합 가능)
    public float effectInterval;       //주기
    public float areaDuration;      // 장판 지속 시간

    //버프형
    public BuffStatType buffStatType; 
    public float buffAmount;
    public float buffDuration;
}

////확장, 유지보수, 가독성, 초기화    
//public enum SKILLDATA
//{
//    A, B, C, D, E, F,
//}
//public class TestData:MonoBehaviour
//{
//    public float[] a1 = new float[(int)SKILLDATA.A];
//}