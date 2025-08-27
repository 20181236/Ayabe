using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class LeaderMovement : MonoBehaviour
{
    public Cinemachine.CinemachineDollyCart dollyCart;
    public float acceleration = 10f;
    public float maxSpeed = 50f;

    private bool followDolly = false;

    public void StartFollowDolly()
    {
        followDolly = true;
    }

    void Update()
    {
        if (followDolly && dollyCart != null)
        {
            dollyCart.m_Speed += acceleration * Time.deltaTime;
            dollyCart.m_Speed = Mathf.Min(dollyCart.m_Speed, maxSpeed);
            // 캐릭터 위치를 Dolly Cart 위치로 이동
            transform.position = dollyCart.transform.position;
            transform.rotation = dollyCart.transform.rotation;
        }
    }
}

