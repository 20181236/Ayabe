using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public void ApplyBuff(Buff buff, System.Action<Buff> onTick)
    {
        if (buff.applyType == BuffApplyType.Tick && buff.tickInterval > 0f)
        {
            StartCoroutine(BuffTickCoroutine(buff, onTick));
        }
        else
        {
            //즉시 적용하거나 duration이 없는 버프 처리
            onTick?.Invoke(buff);
        }
    }

    private IEnumerator BuffTickCoroutine(Buff buff, System.Action<Buff> onTick)
    {
        float elapsed = 0f;
        while (elapsed < buff.duration)
        {
            onTick?.Invoke(buff);
            yield return new WaitForSeconds(buff.tickInterval);
            elapsed += buff.tickInterval;
        }
    }
}
