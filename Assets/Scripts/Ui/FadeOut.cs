using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image Image;
    public float FadeOutTime;

    private Color startColor;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
        startColor = Image.color;    
    }

    private void Update()
    {
        Image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Max(startColor.a * (1 - (Time.time - startTime)), 0));
    }
}
