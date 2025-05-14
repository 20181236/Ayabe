using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class Enemy : Enemy_base
{
    public override void SetData(EnemyData data)
    {
        base.SetData(data);
        // Boss 전용 데이터 처리 가능
    }
}
