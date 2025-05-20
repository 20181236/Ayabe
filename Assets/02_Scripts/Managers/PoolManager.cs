using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public class PoolObject
    {
        public string poolKey; // 딕셔너리 key 역할
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
            // 리스트에서 비활성화된 오브젝트를 찾아 사용
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
