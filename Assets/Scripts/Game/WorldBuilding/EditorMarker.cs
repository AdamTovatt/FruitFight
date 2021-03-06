using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMarker : MonoBehaviour
{
    public Transform MarkerGraphic;
    public MeshRenderer GraphicRenderer;

    private SizeSine sizeSine;

    private void Awake()
    {
        sizeSine = gameObject.GetComponentInChildren<SizeSine>();
    }

    public void SetMarkerSize(Vector2Int size)
    {
        sizeSine.BaseScale = size.x / 4f;
        MarkerGraphic.localPosition = new Vector3((float)size.x / 2f, 0, (float)size.y / 2f);
    }

    public void Hide()
    {
        GraphicRenderer.enabled = false;
    }

    public void Show()
    {
        GraphicRenderer.enabled = true;
    }
}
