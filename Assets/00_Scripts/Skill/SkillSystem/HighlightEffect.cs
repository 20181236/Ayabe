using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEffect : MonoBehaviour
{
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
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i].material.HasProperty("_Color"))
                continue;

            renderers[i].material.color = enable ? highlightColor : originalColors[i];
        }
    }
}
