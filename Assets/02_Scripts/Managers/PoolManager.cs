using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public class PoolObject
    {
        public string poolKey; // ��ųʸ� key ����
    }
    public static PoolManager instance;

    private Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    public void CreatePool(GameObject prefab, int amount)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new List<GameObject>();
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject gameObject = Instantiate(prefab);
            gameObject.SetActive(false);
            gameObject.GetComponent<PoolObject>().poolKey = key;
            poolDictionary[key].Add(gameObject);
        }
    }

    public GameObject GetPool(string key, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(key))
        {
            // ����Ʈ���� ��Ȱ��ȭ�� ������Ʈ�� ã�� ���
            foreach (GameObject gameObject in poolDictionary[key])
            {
                if (!gameObject.activeInHierarchy)
                {
                    gameObject.transform.SetPositionAndRotation(position, rotation);
                    gameObject.SetActive(true);
                    return gameObject;
                }
            }

            Debug.LogWarning($"No inactive objects available in pool: {key}");
        }
        else
        {
            Debug.LogWarning($"Pool for {key} not found!");
        }

        return null;
    }

    public void ReturnPool(GameObject obj)
    {
        PoolObject poolObj = gameObject.GetComponent<PoolObject>();
        if (poolObj == null || string.IsNullOrEmpty(poolObj.poolKey))
        {
            Debug.LogWarning("Returned object is not managed by pool.");
            return;
        }

        gameObject.SetActive(false);
    }
}
