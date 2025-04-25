using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePort;

    public bool isLook;

    Vector3 lookVec;
    Vector3 tauntVec;

    private void Awake()
    {
        
    }
}
