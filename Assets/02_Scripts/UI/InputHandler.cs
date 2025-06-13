using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance { get; private set; }

    public event Action<Vector3> OnGroundClick;
    public event Action<GameObject> OnUnitClick;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<PlayableBase>(out PlayableBase playable))
                {
                    OnUnitClick?.Invoke(playable.gameObject);
                }
                else if (hit.collider.TryGetComponent<EnemyBase>(out EnemyBase enemy))
                {
                    OnUnitClick?.Invoke(enemy.gameObject);
                }
                else
                {
                    OnGroundClick?.Invoke(hit.point);
                }
            }
        }
    }
}
