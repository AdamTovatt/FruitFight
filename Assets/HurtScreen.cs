using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtScreen : MonoBehaviour
{
    public Image ScreenImage;

    private float currentTransparency;
    private float transparencyDelta;
    private bool isLerping;

    private void Start()
    {
        ScreenImage.color.SetAlpha(0);
    }

    private void Update()
    {
        if (isLerping)
        {
            currentTransparency += transparencyDelta * Time.deltaTime;

            if (currentTransparency > 1)
            {
                currentTransparency = 1;
                isLerping = false;
            }
            else if (currentTransparency < 0)
            {
                currentTransparency = 0;
                isLerping = false;
            }

            ScreenImage.color = ScreenImage.color.SetAlpha(currentTransparency);
        }
    }

    public void TurnOff(float fadeTime)
    {
        transparencyDelta = -1f / fadeTime;
        isLerping = true;
    }

    public void TurnOn(float fadeTime)
    {
        transparencyDelta = 1f / fadeTime;
        isLerping = true;
    }
}
