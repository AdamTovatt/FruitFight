using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public float WobbleDuration;
    public float WobbleSpeed;
    public float WobbleAmplitude;
    public float StartOffset;

    private bool wobbleIsOn = false;
    private float wobbleStartTime;
    private Vector3 originalSize;

    private void Update()
    {
        if (wobbleIsOn)
        {
            float x = Time.time - wobbleStartTime;
            if (x < WobbleDuration)
            {
                float sizeMultiplier = ((Mathf.Sin(x * WobbleSpeed + StartOffset * Mathf.PI * 2) * Mathf.Pow(2.71828f, Mathf.Pow(x, 2) * -(4 / WobbleDuration)) * WobbleAmplitude)) + 1;
                transform.localScale = originalSize * sizeMultiplier;
            }
            else
            {
                wobbleIsOn = false;
                transform.localScale = originalSize;
            }
        }
    }

    public void StartWobble()
    {
        if (!wobbleIsOn)
            originalSize = transform.localScale;

        wobbleStartTime = Time.time;
        wobbleIsOn = true;
    }

    public void StopWobble()
    {
        wobbleIsOn = false;
        transform.localScale = originalSize;
    }
}
