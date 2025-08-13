using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public event Action<Buff> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;

    private List<Buff> activeBuffs = new List<Buff>();

    public void ApplyBuff(BuffData data, CharacterBase owner, CharacterBase caster)
    {
        // 버프를 받는 캐릭터(owner)의 BuffManager를 사용하도록 수정
        // 이 코드는 현재 BuffManager에 있는 로직을 그대로 사용하면서
        // 기존 버프가 있는지 확인하고, 없으면 새로 추가합니다.
        Buff existingBuff = activeBuffs.Find(b => b.buffId == data.buffId);

        if (existingBuff != null)
        {
            // 기존 버프가 있으면 갱신
            existingBuff.duration = data.duration;
            existingBuff.value = data.value;
            Debug.Log($"버프 갱신: {existingBuff.buffId} on {owner.name}");
        }
        else
        {
            // 새로운 버프 생성 및 적용
            Buff buff = BuffFactory.CreateBuffFromData(data);
            buff.SetOwner(owner); // 버프를 받는 캐릭터
            buff.caster = caster; // 버프를 건 캐릭터

            // activeBuffs 리스트에 추가
            activeBuffs.Add(buff);

            // 이벤트 발생
            OnBuffAdded?.Invoke(buff);

            // 캐릭터의 스탯 업데이트
            owner.ApplyBuff(data, caster);

            // 버프 지속 시간 코루틴 시작
            StartCoroutine(RemoveBuffAfterDuration(buff));
        }
    }

    private IEnumerator RemoveBuffAfterDuration(Buff buff)
    {
        yield return new WaitForSeconds(buff.duration);

        // 버프 제거
        if (activeBuffs.Contains(buff))
        {
            activeBuffs.Remove(buff);
            OnBuffRemoved?.Invoke(buff);
            buff.owner.RecalculateBuffedStats();
            Debug.Log($"버프 종료: {buff.buffId} on {buff.owner.name}");
        }
    }

    public CharacterBase GetOwnerOfBuff(Buff buff)
    {
        return buff.owner;
    }
}