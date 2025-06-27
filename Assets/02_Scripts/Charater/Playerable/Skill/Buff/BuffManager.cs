//using System.Collections.Generic;
//using UnityEngine;

//public class BuffManager : MonoBehaviour
//{
//    private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

//    private void Update()
//    {
//        float deltaTime = Time.deltaTime;

//        for (int i = activeBuffs.Count - 1; i >= 0; i--)
//        {
//            ActiveBuff buff = activeBuffs[i];
//            buff.OnUpdate(gameObject, deltaTime);
//            buff.timeRemaining -= deltaTime;

//            if (buff.timeRemaining <= 0f)
//            {
//                buff.OnRemove(gameObject);
//                activeBuffs.RemoveAt(i);
//            }
//        }
//    }

//    public void ApplyBuff(InterfaceBuff buff)
//    {
//        if (buff == null)
//        {
//            Debug.LogWarning("Trying to apply null buff.");
//            return;
//        }

//        ActiveBuff newBuff = new ActiveBuff(buff, buff.Duration);
//        newBuff.OnApply(gameObject);
//        activeBuffs.Add(newBuff);
//    }
//}
