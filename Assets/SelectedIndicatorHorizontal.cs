using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedIndicatorHorizontal : MonoBehaviour
{
    public Image RightMarker;
    public Image LeftMarker;

    public void SetContentSize(Vector2 size)
    {
        RightMarker.transform.position = new Vector3(transform.position.x + (size.x / 2), transform.position.y, transform.position.z);
        LeftMarker.transform.position = new Vector3(transform.position.x - (size.x / 2), transform.position.y, transform.position.z);
    }
}
