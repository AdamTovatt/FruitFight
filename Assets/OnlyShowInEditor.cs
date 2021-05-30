using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyShowInEditor : MonoBehaviour
{
    void Start()
    {
        if (!WorldBuilder.IsInEditor)
            gameObject.SetActive(false);
    }
}
