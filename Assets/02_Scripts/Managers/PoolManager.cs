using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PoolObjectInfo
{
    //-----------물론 클래스로 들어가는게 좋긴한데 
    //public string key;//"Enemy","Bullet"
    //public GameObject originalPrefab;
    //public int initialSize;
    //public List<GameObject> poolObjectList = new List<GameObject>();
    //public Dictionary<?,?> poolObjectDictionary=;
}
public class PoolManager : MonoBehaviour
{
    public enum PoolType
    {
        Playable,
        Enemy,
        PlayableBullet,
        EnemyBullet,
        Effect,
    }
    public static PoolManager instance;

    private Dictionary<PoolType, Dictionary<bool, Bullet>> poolDictionary = new Dictionary<PoolType, Dictionary<bool, Bullet>>();

    private void Awake()
    {
        instance = this;
    }

}
