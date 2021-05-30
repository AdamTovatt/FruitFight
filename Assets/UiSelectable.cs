using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiSelectable : MonoBehaviour, ISelectHandler
{
    public GameObject SelectedMarkerPrefab;
    public Vector2 ContentSize = new Vector2(300, 50);
    
    private GameObject selectedMarker = null;
    private float canvasScaleFactor = 1f;

    private void Start()
    {
        WorldEditorUi.Instance.RegisterSelectable(this);
    }

    public void WasDeselected()
    {
        if (selectedMarker != null)
            Destroy(selectedMarker);
    }

    public void OnSelect(BaseEventData eventData)
    {
        canvasScaleFactor = WorldEditorUi.Instance.GetComponent<Canvas>().transform.localScale.magnitude;
        WorldEditorUi.Instance.SelectableWasSelected(this);
        selectedMarker = Instantiate(SelectedMarkerPrefab, transform.position, transform.rotation, transform);
        selectedMarker.GetComponent<SelectedIndicatorHorizontal>().SetContentSize(ContentSize * canvasScaleFactor);
    }
}
