using UnityEngine;

public class SkillRangeVisualizer : MonoBehaviour
{
    private static SkillRangeVisualizer instance;

    [SerializeField] private SpriteRenderer rangeSprite;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    public static void Show(float radius, Vector3 position)
    {
        if (instance == null)
            return;

        float scale = radius * 2f; // 직경 기준
        instance.rangeSprite.transform.position = position;
        instance.rangeSprite.transform.localScale = new Vector3(scale, scale, 1f);
        instance.rangeSprite.gameObject.SetActive(true);
    }

    public static void UpdatePosition(Vector3 position)
    {
        if (instance == null)
            return;
        instance.rangeSprite.transform.position = position;
    }

    public static void Hide()
    {
        if (instance == null) return;
        instance.rangeSprite.gameObject.SetActive(false);
    }
}
