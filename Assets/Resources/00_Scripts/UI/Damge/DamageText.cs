using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float floatSpeed = 30f;
    public float lifeTime = 1f;

    private float timer = 0f;

    public void Setup(int damage)
    {
        text.text = damage.ToString();
    }

    void Update()
    {
        // 위로 이동
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
