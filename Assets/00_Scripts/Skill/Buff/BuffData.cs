using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "Buffs/BuffData")]
public class BuffData : ScriptableObject, InterfaceBuff
{
    [Header("기본 정보")]
    [SerializeField] private string _buffId;
    [SerializeField] private BuffGroup _group;
    [SerializeField] private BuffCategory _category;
    [SerializeField] private BuffApplyType _applyType;
    [SerializeField] private BuffStatType _targetStat;

    [Header("버프 수치")]
    [SerializeField] private float _value; // 0.1f == +10%
    [SerializeField] private float _duration = 5f;
    [SerializeField] private float _tickInterval = 1f;

    public string buffId => _buffId;
    public BuffGroup group => _group;
    public BuffCategory category => _category;
    public BuffApplyType applyType => _applyType;
    public BuffStatType targetStat => _targetStat;
    public float value => _value;
    public float duration => _duration;
    public float tickInterval => _tickInterval;

    public void SetData(BuffStatType stat, float value, float duration, BuffApplyType applyType, float tickInterval)
    {
        _targetStat = stat;
        _value = value;
        _duration = duration;
        _applyType = applyType;
        _tickInterval = tickInterval;
    }
}
