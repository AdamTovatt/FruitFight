using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiSelectable : MonoBehaviour, ISelectHandler
{
    public GameObject SelectedMarkerPrefab;
    private GameObject selectedMarker = null;

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
        WorldEditorUi.Instance.SelectableWasSelected(this);
        selectedMarker = Instantiate(SelectedMarkerPrefab, transform.position, transform.rotation, transform);
        Debug.Log(eventData.selectedObject.transform.name);
    }
}
