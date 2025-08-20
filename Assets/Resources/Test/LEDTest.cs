using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDTest : MonoBehaviour
{
    public Material ledMaterial;       // LED 머티리얼
    public float blinkSpeed = 30f;      // 깜빡임 속도
    public Color baseColor = Color.cyan; // 기본 LED 색상

    void Update()
    {
        // 깜빡임 구현
        float intensity = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        ledMaterial.SetColor("_EmissionColor", baseColor * intensity);

        // 텍스처 UV 이동 (흐르는 LED 효과)
        Vector2 offset = new Vector2(Time.time * 0.2f, 0);
        ledMaterial.SetTextureOffset("_MainTex", offset);
        ledMaterial.SetTextureOffset("_EmissionMap", offset);
    }
}
