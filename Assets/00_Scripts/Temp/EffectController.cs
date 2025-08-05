using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public static EffectController instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void PlayEffect(EffectId effectId, Vector3 position, float duration = 1f)
    {
        string path = GetEffectPath(effectId);
        Debug.Log($"[EffectController] Try to load effect: {effectId}, path: {path}");

        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"[EffectController] Effect prefab NOT FOUND at path: {path}");
            return;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        Debug.Log($"[EffectController] Effect instantiated at: {position}");

        StartCoroutine(DestroyAfterRealtime(instance, duration));
    }

    private IEnumerator DestroyAfterRealtime(GameObject gameObject, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Destroy(gameObject);
    }

    private string GetEffectPath(EffectId effectId)
    {
        switch (effectId)
        {
            case EffectId.Ludo_CastEffect:
                return  "Art/Effect/Ludo_CastEffect/Buff_03a";
            case EffectId.Luna_CastEffect:
                return "Art/Effect/Luna_CastEffect";
            // 추가 이펙트 경로 계속 작성
            default:
                Debug.LogWarning($"[EffectController] No path mapped for effectId: {effectId}");
                return null;
        }
    }
}
