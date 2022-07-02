using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedIndicatorHorizontal : SelectedIndicatorBase
{
    public Image RightMarker;
    public Image LeftMarker;

    public override void SetContentSize(Vector2 size, Vector2 pivot, Vector2 offset)
    {
        RightMarker.transform.localPosition = new Vector3(size.x * (1f - pivot.x) + offset.x * size.x, transform.localPosition.y + offset.y * size.y, transform.localPosition.z);
        LeftMarker.transform.localPosition = new Vector3(size.x * -pivot.x + offset.x * size.x, transform.localPosition.y + offset.y * size.y, transform.localPosition.z);
    }
}
