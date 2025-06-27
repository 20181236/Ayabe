//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ActiveBuff
//{
//    public InterfaceBuff buff;     // 어떤 버프인지
//    public float timeRemaining;    // 남은 시간

//    public ActiveBuff(InterfaceBuff buff, float duration)
//    {
//        this.buff = buff;
//        this.timeRemaining = duration;
//    }

//    public float Duration => buff.Duration;

//    //I try Lamda...
//    public void OnApply(GameObject target) => buff.OnApply(target);
//    public void OnRemove(GameObject target) => buff.OnRemove(target);
//    public void OnUpdate(GameObject target, float deltaTime) => buff.OnUpdate(target, deltaTime);

//    //public void OnApply(GameObject target)
//    //{
//    //    buff.OnApply(target);
//    //}

//    //public void OnRemove(GameObject target)
//    //{
//    //    buff.OnRemove(target);
//    //}

//    //public void OnUpdate(GameObject target, float deltaTime)
//    //{
//    //    buff.OnUpdate(target, deltaTime);
//    //}
//}
