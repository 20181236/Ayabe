using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennsyutu : MonoBehaviour
{
    public Light[] pointLights; // 8개의 PointLight를 Inspector에서 할당
    public float delay = 0.5f;  // 켜지는 시간 간격

    private void Start()
    {
        // 처음에는 모두 끔
        foreach (var light in pointLights)
            light.enabled = false;

        // 연출 시작
        StartCoroutine(TurnOnLightsSequentially());
    }

    private IEnumerator TurnOnLightsSequentially()
    {
        foreach (var light in pointLights)
        {
            light.enabled = true;
            yield return new WaitForSeconds(delay); // 다음 라이트까지 대기
        }
    }
}
