using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEffect : MonoBehaviour
{
    public GameObject highlightPrefab;
    private GameObject highlightInstance;

    private Renderer[] renderers;
    private Color[] originalColors;

    public Color highlightColor = Color.white;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
        }
    }

    public void SetHighlight(bool enable)
    {
        if (enable && highlightInstance == null)
        {
            highlightInstance = Instantiate(highlightPrefab, transform);
            highlightInstance.transform.localPosition = Vector3.zero; // ¹ß ¹Ø À§Ä¡
        }
        else if (!enable && highlightInstance == null)
        {
            Debug.LogWarning("Highlight prefab is missing!");
        }
        else if (!enable && highlightInstance != null)
        {
            Destroy(highlightInstance);
        }
    }
}
