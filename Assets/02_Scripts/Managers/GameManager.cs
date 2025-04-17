using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //가장 가까운 적을 지정? 저장? 해줄 변수가 필요할지도
    public float targettingEnemy;

    public Transform charaterPosition;
    private static GameManager instance = null;
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

    public static GameManager Instance
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

    //그걸 찾아주는 함수도 필요할지도
    public void Seach()
    {

    }
}
