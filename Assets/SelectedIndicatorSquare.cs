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

    public override void SetContentSize(Vector2 size, Vector2 pivot)
    {
        TopRightCorner.transform.localPosition = new Vector3(size.x * (1f - pivot.x), transform.localPosition.y, transform.localPosition.z);
        TopLeftCorner.transform.localPosition = new Vector3(size.x * -pivot.x, transform.localPosition.y, transform.localPosition.z);

        BottomRightCorner.transform.localPosition = new Vector3(size.x * (1f - pivot.x), -size.y * pivot.y, transform.localPosition.z);
        BottomLeftCorner.transform.localPosition = new Vector3(size.x * -pivot.x, -size.y * pivot.y, transform.localPosition.z);
    }
}
