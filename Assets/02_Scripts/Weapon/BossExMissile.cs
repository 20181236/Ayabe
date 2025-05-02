using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExMissile : MonoBehaviour
{
    public Animator animator;
    public GameObject subMissilePrefab;
    public Vector3 targetPosition;
    private bool hasSplit = false;  // Split�� �̹� ����Ǿ����� üũ�ϴ� ����

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Vector3 target)
    {
        targetPosition = target;
    }

    public void Split()
    {
        if (hasSplit) return;  // �̹� Split�� ����Ǿ����� ����

        hasSplit = true;  // Split�� �� �� ���������Ƿ� �÷��� ����

        Debug.Log("Split");

        // ���� �̻��� ����
        for (int i = 0; i < 5; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2f;
            Vector3 subTarget = new Vector3(targetPosition.x + offset.x, 10f, targetPosition.z + offset.y);
            GameObject sub = Instantiate(subMissilePrefab, transform.position, Quaternion.identity);
            sub.GetComponent<BossExSubMissile>().Init(subTarget);
        }

        // �̻��� ��ü ����
        Destroy(gameObject);
    }
}
