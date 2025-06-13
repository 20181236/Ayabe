using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public static Targeting instance { get; private set; }

    private Action<Vector3> positionCallback;
    private Action<GameObject> unitCallback;

    private void Awake()
    {
        instance = this;
    }

    public void RequestPosition(Action<Vector3> callback)
    {
        positionCallback = callback;
        InputHandler.instance.OnGroundClick += OnGroundClicked;
    }

    public void RequestUnit(Action<GameObject> callback, Predicate<GameObject> filter)
    {
        unitCallback = unit =>
        {
            if (filter(unit))
            {
                callback?.Invoke(unit);
                InputHandler.instance.OnUnitClick -= OnUnitClicked;  // ��� ����
                unitCallback = null;
            }
            else
            {
                Debug.Log("�߸��� Ÿ���Դϴ�.");
            }
        };
        InputHandler.instance.OnUnitClick += OnUnitClicked;
    }

    private void OnGroundClicked(Vector3 pos)
    {
        InputHandler.instance.OnGroundClick -= OnGroundClicked;
        positionCallback?.Invoke(pos);
        positionCallback = null;
    }

    private void OnUnitClicked(GameObject unitObj)
    {
        InputHandler.instance.OnUnitClick -= OnUnitClicked;

        unitCallback?.Invoke(unitObj);

        unitCallback = null;
    }
}
