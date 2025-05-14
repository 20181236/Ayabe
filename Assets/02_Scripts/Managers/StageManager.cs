using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; }

    public int totalWaves;
    public bool isStageClear = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (SpawnManager.instance != null)
        {
            totalWaves = SpawnManager.instance.waves.Length;
            SpawnManager.instance.StartWave(); // ù ���̺� �ڵ� ����
        }
        else
        {
            Debug.LogError("SpawnManager�� �������� �ʽ��ϴ�.");
        }
    }

    private void Update()
    {

    }

}
