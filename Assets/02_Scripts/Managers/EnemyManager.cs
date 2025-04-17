using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> enemies = new List<Enemy>();//에너미 리스트 관리 OK

    public float detectionRange = 10f; // 탐지 거리 OK
    public string enemyTag = "Enemy"; // 감지할 태그 ????????????? 태그로감지?

    public LayerMask detectionLayer; // 감지할 레이어 레이어감지 OK

    private static EnemyManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static EnemyManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void RegisterEnemy(Enemy enemy)//생성함수 OK
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy)//죽였을대 제거함수 OK
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    //내가 만든 코드가아님 gpt돌림 이상할수있음
    //distance가 안쓰임
    //필요없는게 있음, 레이어를 나누냐 안나누냐에따라
    public Enemy GetClosestEnemy(Vector3 fromCharaterPosition)//이해안됨
    {
        Enemy closest = null;//처음 초기화 OK
        float minDist = detectionRange * detectionRange;//sqrMagnitude로 비교할 거니까 거리 제곱을 저장 OK

        foreach (var enemy in enemies)//여기서부터 탐색하면서 돌린다는 뜻 OK
        {
            if (enemy == null || )//뒤지면 넘김OK
                continue;

            if (!enemy.CompareTag(enemyTag))//Enemy태그 아니면 무시 OK
                continue;

            if (((1 << enemy.gameObject.layer) & detectionLayer) == 0)//이해는 안되지만 레이어 탐지 '공식'???? 비트민거임 고쳐야됨 레이어가 4랑같으면 이걸처리해
                continue;

            float dist = (enemy.transform.position - fromCharaterPosition).sqrMagnitude;//벡터 뺄셈은 알겠음, 이것보다 distance를 사용하지않은 즉 거리를 저장하는 

            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
