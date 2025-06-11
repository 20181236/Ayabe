using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    public int maxMana = 10;
    private int currentMana;

    public event Action<float> OnManaChanged;

    public float regenInterval = 1f;
    private float regenTimer = 0f;
    public int regenAmount = 1;
    public static ManaManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        currentMana = 0;
        OnManaChanged?.Invoke(1f);
    }

    private void Update()
    {
        regenTimer += Time.deltaTime;
        while (regenTimer >= regenInterval)
        {
            regenTimer -= regenInterval;
            RestoreMana(regenAmount);//자연 마나회복 1
        }
    }
    public bool UseMana(int cost)
    {
        if (currentMana >= cost)
        {
            currentMana -= cost;
            OnManaChanged?.Invoke((float)currentMana / maxMana);
            return true;//마나가 사용됨을 반환해야 그 스킬을 비활성화 하든말든 하나?
        }
        return false;
    }
    public void RestoreMana(int amount)
    {
        //나중에 마나채워주는 스킬도 만들어 볼 예정
        int oldMana = currentMana;
        currentMana = Mathf.Min(currentMana + amount, maxMana);

        if (currentMana != oldMana)
        {
            OnManaChanged?.Invoke((float)currentMana / maxMana);
        }
    }
    public int GetCurrentMana() => currentMana;
}
