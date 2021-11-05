using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraViewBlocker : MonoBehaviour
{
    public Image FullBlocker;
    public Image LeftBlocker;
    public Image RightBlocker;

    public void ActivateBlocker(CameraViewType viewType)
    {
        if (viewType == CameraViewType.Full)
            FullBlocker.gameObject.SetActive(true);
        if (viewType == CameraViewType.Left)
            LeftBlocker.gameObject.SetActive(true);
        if (viewType == CameraViewType.Right)
            RightBlocker.gameObject.SetActive(true);
    }

    public void DeactivateBlocker(CameraViewType viewType)
    {
        if (viewType == CameraViewType.Full)
            FullBlocker.gameObject.SetActive(false);
        if (viewType == CameraViewType.Left)
            LeftBlocker.gameObject.SetActive(false);
        if (viewType == CameraViewType.Right)
            RightBlocker.gameObject.SetActive(false);
    }
}
