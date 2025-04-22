using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //가장 가까운 적을 지정? 저장? 해줄 변수가 필요할지도
    public Transform charaterPosition;

    public static GameManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Enemy GetNearestEnemyToPosition(Vector3 fromPosition)
    {
        var enemies = EnemyManager.instance.GetEnemies();
        Debug.Log(enemies);
        Debug.Log(enemies.Count);
        if (enemies == null || enemies.Count == 0)
            return null;

        Enemy nearest = null;//가장 가까운 적을 저장할 변수 선언(초기값은 null)
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(fromPosition, enemy.transform.position);
            
            if (dist < minDistance)//어떤놈이 나에게 가까운지 없음 거리만 체크하고 1번2번 누가 더 가까운지 가장가까운녀석이누구야
            {
                minDistance = dist;
                nearest = enemy;
                Debug.Log(enemy.name + dist);
            }
        }
        Debug.Log(nearest);
        return nearest;
    }
}
