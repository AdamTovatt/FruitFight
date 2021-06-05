using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiSelectable : MonoBehaviour, ISelectHandler
{
    public GameObject SelectedMarkerPrefab;
    public bool UseCustomContentSize = false;
    public Vector2 ContentSize = new Vector2(300, 50);
    public bool UseCustomPivot = false;
    public Vector2 Pivot = new Vector2(0.5f, 0.5f);
    
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
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        WorldEditorUi.Instance.SelectableWasSelected(this);
        selectedMarker = Instantiate(SelectedMarkerPrefab, transform.position, transform.rotation, transform);
        selectedMarker.GetComponent<SelectedIndicatorBase>().SetContentSize(UseCustomContentSize ? ContentSize : rect.sizeDelta, UseCustomPivot ? Pivot : rect.pivot);
    }
}
