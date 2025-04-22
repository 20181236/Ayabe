using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void Start()
    {
        EnemyManager.instance.RegisterEnemy(this);
        Debug.Log("µî·ÏµÊ");
    }

    private void OnDestroy()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(this);
    }
}
