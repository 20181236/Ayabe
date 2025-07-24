using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyNokori : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemyCountText;

    public void UpdateEnemyCount(int current, int total)
    {
        enemyCountText.text = $"³²Àº Àû: {current} / {total}";
    }
}
