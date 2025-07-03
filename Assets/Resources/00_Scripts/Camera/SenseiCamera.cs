using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SenseiCamera : MonoBehaviour
{
    public float baseDistance = 50f;
    public float maxBaseDistance=100f;
    public float distanceMultiplier = 0.5f;
    public Vector3 cameraOffset = new Vector3(20, 5, -10);

}
    