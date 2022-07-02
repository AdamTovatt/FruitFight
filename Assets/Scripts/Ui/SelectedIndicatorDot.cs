using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedIndicatorDot : SelectedIndicatorBase
{
    public Image Dot;

    public override void SetContentSize(Vector2 size, Vector2 pivot, Vector2 offset)
    {
        Dot.rectTransform.sizeDelta = size;
        Dot.rectTransform.localPosition = new Vector3(size.x * pivot.x + offset.x * size.x, size.y * pivot.y + offset.y * size.y);
    }
}
