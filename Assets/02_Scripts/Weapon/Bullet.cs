using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //쓸모없는 상속을 하고있음

    public int damage;//이게 캐릭터마다 데미지가 다를꺼고 버프나 이런거 맥이면 더쌔져야하는데 어떻하지?

    void OnCollisionEnter(Collision collision)
    {

    }

    void OnTriggerEnter(Collider other)
    {

    }
}
