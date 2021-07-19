using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedIndicatorDot : SelectedIndicatorBase
{
    public Image Dot;

    public override void SetContentSize(Vector2 size, Vector2 pivot)
    {
        Dot.rectTransform.sizeDelta = size;
        Dot.rectTransform.localPosition = new Vector3(size.x * pivot.x, size.y * pivot.y);
    }
}
