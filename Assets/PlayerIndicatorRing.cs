using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerIndicatorRing : MonoBehaviour
{
    public static List<PlayerIndicatorRing> Rings = new List<PlayerIndicatorRing>();

    private Renderer[] renderers;
    private PlayerIndicatorRing otherRing;

    public void Initialize(bool playerOne)
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>();
        Rings.Add(this);

        if (playerOne)
        {
            SetLayer("PlayerOne");

        }
        else
        {
            SetLayer("PlayerTwo");
        }

        SetAlpha(0);
    }

    private void SetLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        foreach (Renderer renderer in renderers)
        {
            renderer.gameObject.layer = layer;
        }
    }

    private void OnDestroy()
    {
        Rings.Remove(this);
    }

    private void Update()
    {
        if (Rings.Count > 0)
        {
            if (otherRing == null)
                otherRing = Rings.Where(x => x != this).FirstOrDefault();

            if (otherRing != null)
            {
                float alpha = 1f - Mathf.Clamp((otherRing.transform.position - transform.position).sqrMagnitude / 7f, 0, 1);

                SetAlpha(alpha);
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }
}
