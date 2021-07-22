using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyShowInEditor : MonoBehaviour
{
    void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
