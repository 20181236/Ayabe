using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public enum InGameState
    {
        Prepare,
        Battle,
        End
    }
    public InGameState inGameState;
    private float prepareTime = 1f; 

    private void Awake()
    {
        inGameState= InGameState.Prepare;
        EnemyManager.instance.SpawnEnemy(EnemyType.Normal, new Vector3(0, 0, 5));//�̰ɷ� ��ȯ�ϸ��
    }
    private void Start()
    {
        StartCoroutine(StartGame());
    }
    private void Update()
    {
        if (inGameState != InGameState.Battle)
            return;
        if (!EnemyManager.instance.HasEnemyOfType(EnemyType.Boss))
        {
            EndGame();
            return;
        }
         if (!EnemyManager.instance.HasEnemy())
        {
            EndGame();
            return;
        }
    }
    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(prepareTime);
        inGameState = InGameState.Battle;
    }

    void EndGame()
    { 
        inGameState = InGameState.End;
        //���⿡ ���� ���� ���� ������ �־����
    }
}
