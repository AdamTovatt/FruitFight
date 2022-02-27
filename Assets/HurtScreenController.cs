using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtScreenController : MonoBehaviour
{
    public HurtScreen FullScreen;
    public HurtScreen LeftScreen;
    public HurtScreen RightScreen;

    public void ActivateScreen(CameraViewType viewType, float fadeTime)
    {
        if (viewType == CameraViewType.Full)
            FullScreen.TurnOn(fadeTime);
        if (viewType == CameraViewType.Left)
            LeftScreen.TurnOn(fadeTime);
        if (viewType == CameraViewType.Right)
            RightScreen.TurnOn(fadeTime);
    }

    public void DeactivateScreen(CameraViewType viewType, float fadeTime)
    {
        if (viewType == CameraViewType.Full)
            FullScreen.TurnOff(fadeTime);
        if (viewType == CameraViewType.Left)
            LeftScreen.TurnOff(fadeTime);
        if (viewType == CameraViewType.Right)
            RightScreen.TurnOn(fadeTime);
    }

    public void FlashScreen(CameraViewType viewType, float duration)
    {
        float upTime = Mathf.Min(0.3f, duration * 0.5f);
        float downTime = duration - upTime;

        ActivateScreen(viewType, upTime);
        this.CallWithDelay(() => { DeactivateScreen(viewType, duration); }, upTime);
    }
}
