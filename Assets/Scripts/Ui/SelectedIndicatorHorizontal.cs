using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedIndicatorHorizontal : MonoBehaviour
{
    public Image RightMarker;
    public Image LeftMarker;

    public void SetContentSize(Vector2 size, Vector2 pivot)
    {
        RightMarker.transform.localPosition = new Vector3(size.x * (1f - pivot.x), transform.localPosition.y, transform.localPosition.z);
        LeftMarker.transform.localPosition = new Vector3(size.x * -pivot.x, transform.localPosition.y, transform.localPosition.z);
    }
}
