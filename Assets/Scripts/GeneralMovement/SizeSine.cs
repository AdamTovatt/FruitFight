using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSine : MonoBehaviour
{
    public float Speed = 5f;
    public float SizeVariation = 0.5f;

    public float BaseScale = 1f;

    private void Update()
    {
        float newScale = BaseScale + Mathf.Sin(Time.time * Speed) * SizeVariation;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
