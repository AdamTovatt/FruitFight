using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedIndicatorSquare : SelectedIndicatorBase
{
    public Image TopLeftCorner;
    public Image TopRightCorner;
    public Image BottomRightCorner;
    public Image BottomLeftCorner;

    public override void SetContentSize(Vector2 size, Vector2 pivot, Vector2 offset)
    {
        TopRightCorner.transform.localPosition = new Vector3(size.x * (1f - pivot.x) + offset.x * size.x, transform.localPosition.y + offset.y * size.y, transform.localPosition.z);
        TopLeftCorner.transform.localPosition = new Vector3(size.x * -pivot.x + offset.x * size.x, transform.localPosition.y + offset.y * size.y, transform.localPosition.z);

        BottomRightCorner.transform.localPosition = new Vector3(size.x * (1f - pivot.x) + offset.x * size.x, -size.y * pivot.y + offset.y * size.y, transform.localPosition.z);
        BottomLeftCorner.transform.localPosition = new Vector3(size.x * -pivot.x + offset.x * size.x, -size.y * pivot.y + offset.y * size.y, transform.localPosition.z);
    }
}
