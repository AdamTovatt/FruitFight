using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMarker : MonoBehaviour
{
    public Transform MarkerGraphic;

    public void SetMarkerSize(Vector2Int size)
    {
        MarkerGraphic.localPosition = new Vector3((float)size.x / 2f, 0, (float)size.y / -2f);
    }
}
