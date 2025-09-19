using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public event Action<Buff, float> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;

    private List<Buff> activeBuffs = new List<Buff>();

    public void ApplyBuff(BuffData data, CharacterBase owner, CharacterBase caster)
    {
        if (data == null) 
            return;

        Buff existingBuff = activeBuffs.Find(b => b.buffId == data.buffId);

        if (existingBuff != null)
        {
            existingBuff.duration = data.duration;
            existingBuff.value = data.value;
            Debug.Log($"[갱신] {existingBuff.buffId} on {owner.name} (시전: {caster.name})");
            owner.RecalculateBuffedStats();
        }
        else
        {
            Buff buff = BuffFactory.CreateBuffFromData(data);
            buff.SetOwner(owner);
            buff.caster = caster;

            activeBuffs.Add(buff);
            if (!owner.activeBuffs.Contains(buff))
                owner.activeBuffs.Add(buff);

            Debug.Log($"[추가] 버프 {buff.buffId} 적용됨 -> {owner.name} (시전: {caster.name})");

            owner.RecalculateBuffedStats();

            //버프UI이벤트
            OnBuffAdded?.Invoke(buff, buff.duration);

            StartCoroutine(BuffRoutine(buff));
        }
    }
    private IEnumerator BuffRoutine(Buff buff)
    {
        buff.owner.RecalculateBuffedStats();

        float elapsed = 0f;
        float interval = Mathf.Max(buff.tickInterval, 0.1f);

        while (elapsed < buff.duration)
        {
            if (buff.applyType == BuffApplyType.Tick || buff.applyType == BuffApplyType.Both)
            {
                buff.owner.ExecuteOnBuffTick(buff);
            }

            yield return new WaitForSeconds(interval);

            elapsed += interval;
        }

        OnBuffRemoved?.Invoke(buff);

        buff.owner.activeBuffs.Remove(buff);
        activeBuffs.Remove(buff);
        buff.owner.RecalculateBuffedStats();
        Debug.Log($"[종료] 버프 {buff.buffId} 종료 -> {buff.owner.name}");
    }
    //private IEnumerator BuffRoutine(Buff buff)
    //{
    //    float elapsed = 0f;

    //    // 즉시 회복 (Burst)
    //    if (buff.applyType == BuffApplyType.Burst || buff.applyType == BuffApplyType.Both)
    //    {
    //        float burstHeal = buff.owner.baseHealPower * buff.value;
    //        buff.owner.Heal(burstHeal);
    //    }

    //    // Tick 반복 회복
    //    if (buff.applyType == BuffApplyType.Tick || buff.applyType == BuffApplyType.Both)
    //    {
    //        // Tick 시작 전 지연
    //        yield return new WaitForSeconds(1f);

    //        float interval = Mathf.Max(buff.tickInterval, 0.1f);
    //        while (elapsed < buff.duration)
    //        {
    //            // Tick 회복은 오직 tickValue만 적용
    //            float tickHeal = buff.owner.baseHealPower * buff.tickValue;
    //            buff.owner.Heal(tickHeal);

    //            yield return new WaitForSeconds(interval);
    //            elapsed += interval;
    //        }
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(buff.duration);
    //    }

    //    // 버프 종료
    //    activeBuffs.Remove(buff);
    //}



    public void OnBuffTick(Buff buff)
    {
        if (buff.targetStat == BuffStatType.HealPower)
        {
            float healAmount = buff.owner.baseHealPower * buff.tickValue;
            buff.owner.Heal(healAmount);
        }
        else if (buff.targetStat == BuffStatType.AttackPower)
        {
            buff.owner.RecalculateBuffedStats();
        }
    }

    public CharacterBase GetOwnerOfBuff(Buff buff)
    {
        return buff.owner;
    }
}

//    // Buff의 지속적인 효과를 처리하는 코루틴 (추가된 부분)
//    private IEnumerator BuffRoutine(Buff buff)
//    {
//        float elapsed = 0f;

//        // Tick 또는 Continuous 타입일 경우 주기적 효과 적용
//        if (buff.applyType == BuffApplyType.Tick || buff.applyType == BuffApplyType.Continuous)
//        {
//            float interval = Mathf.Max(buff.tickInterval, 0.1f);
//            while (elapsed < buff.duration)
//            {
//                yield return new WaitForSeconds(interval);
//                OnBuffTick(buff);
//                elapsed += interval;
//            }
//        }
//        // Burst 타입일 경우 즉시 효과 적용 후 대기
//        else
//        {
//            OnBuffTick(buff);
//            yield return new WaitForSeconds(buff.duration);
//        }
//        //float elapsed = 0f;
//        //Debug.Log($"BuffRoutine 시작: {buff.targetStat}, applyType: {buff.applyType}, duration: {buff.duration}, tickInterval: {buff.tickInterval}");

//        //// 버프 타입에 따라 효과 실행
//        //if (buff.applyType == BuffApplyType.Tick)
//        //{
//        //    float interval = Mathf.Max(buff.tickInterval, 0.1f);
//        //    while (elapsed < buff.duration)
//        //    {
//        //        yield return new WaitForSeconds(interval);
//        //        OnBuffTick(buff);
//        //        elapsed += interval;
//        //    }
//        //}
//        //else if (buff.applyType == BuffApplyType.Continuous)
//        //{
//        //    yield return new WaitForSeconds(buff.duration);
//        //}
//        //else // Burst 타입
//        //{
//        //    OnBuffTick(buff);
//        //    yield return new WaitForSeconds(buff.duration);
//        //}

//        Debug.Log($"BuffRoutine 종료: {buff.targetStat}");

//        // 코루틴 종료 시 버프 제거 및 스탯 재계산
//        activeBuffs.Remove(buff);
//        buff.owner.RecalculateBuffedStats();
//        OnBuffRemoved?.Invoke(buff);
//    }

//    // Buff의 틱 효과를 처리하는 메서드 (추가된 부분)
//    private void OnBuffTick(Buff buff)
//    {
//        switch (buff.targetStat)
//        {
//            case BuffStatType.HealPower:
//                buff.owner.Heal(buff.owner.baseHealPower * buff.value);
//                break;
//                // 다른 BuffStatType에 대한 로직은 여기에 추가
//        }
//    }
//}

////public class BuffManager : MonoBehaviour
////{
////    //public event Action<Buff> OnBuffAdded;
////    public event Action<Buff, float> OnBuffAdded;
////    public event Action<Buff> OnBuffRemoved;
////    //public event Action<Buff> OnBuffRemoved;

////    private List<Buff> activeBuffs = new List<Buff>();

////    public void ApplyBuff(BuffData data, CharacterBase owner, CharacterBase caster)
////    {
////        // 버프를 받는 캐릭터(owner)의 BuffManager를 사용하도록 수정
////        // 이 코드는 현재 BuffManager에 있는 로직을 그대로 사용하면서
////        // 기존 버프가 있는지 확인하고, 없으면 새로 추가합니다.
////        if (data == null)
////        {
////            Debug.Log($"BuffManager: ApplyBuff 호출 시 BuffData가 null입니다. owner={owner.name}");
////            return;
////        }
////        Buff existingBuff = activeBuffs.Find(b => b.buffId == data.buffId);

////        if (existingBuff != null)
////        {
////            // 기존 버프가 있으면 갱신
////            existingBuff.duration = data.duration;
////            existingBuff.value = data.statxValue;
////            Debug.Log($"버프 갱신: {existingBuff.buffId} on {owner.name}");
////        }
////        else
////        {
////            // 새로운 버프 생성 및 적용
////            Buff buff = BuffFactory.CreateBuffFromData(data);
////            buff.SetOwner(owner); // 버프를 받는 캐릭터
////            buff.caster = caster; // 버프를 건 캐릭터

////            // activeBuffs 리스트에 추가
////            activeBuffs.Add(buff);

////            // 이벤트 발생
////            //OnBuffAdded?.Invoke(buff);
////            OnBuffAdded?.Invoke(buff, buff.duration);

////            //// 캐릭터의 스탯 업데이트
////            //owner.ApplyBuff(data, caster);
////            // 캐릭터의 스탯 업데이트
////            owner.RecalculateBuffedStats(); // 버프가 스탯에 영향을 줄 경우 즉시 재계산

////            // 버프 지속 시간 코루틴 시작
////            StartCoroutine(RemoveBuffAfterDuration(buff));
////        }
////    }

////    private IEnumerator RemoveBuffAfterDuration(Buff buff)
////    {
////        yield return new WaitForSeconds(buff.duration);

////        // 버프 제거
////        if (activeBuffs.Contains(buff))
////        {
////            activeBuffs.Remove(buff);
////            OnBuffRemoved?.Invoke(buff);
////            buff.owner.RecalculateBuffedStats();
////            Debug.Log($"버프 종료: {buff.buffId} on {buff.owner.name}");
////        }
////    }

////    public CharacterBase GetOwnerOfBuff(Buff buff)
////    {
////        return buff.owner;
////    }
////}