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
    private UiManager manager { get { if (_manager == null) SetupManager(); return _manager; } set { _manager = value; } }
    private UiManager _manager;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        SetupManager();
    }

    private void Awake()
    {
        SetupManager(true);
    }

    public void WasDeselected()
    {
        if (selectedMarker != null)
        {
            Destroy(selectedMarker);
            selectedMarker = null;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        manager.SelectableWasSelected(this);
        if (selectedMarker == null)
        {
            selectedMarker = Instantiate(SelectedMarkerPrefab, transform.position, transform.rotation, transform);
            selectedMarker.GetComponent<SelectedIndicatorBase>().SetContentSize(UseCustomContentSize ? ContentSize : rect.sizeDelta, UseCustomPivot ? Pivot : rect.pivot);
            AudioManager.Instance.Play("select");
        }
    }

    private void SetupManager(bool ignoreErrors = false)
    {
        if (WorldEditorUi.Instance != null)
        {
            WorldEditorUi.Instance.RegisterSelectable(this);
            manager = WorldEditorUi.Instance;
        }
        else if (MainMenuUi.Instance != null)
        {
            MainMenuUi.Instance.RegisterSelectable(this);
            manager = MainMenuUi.Instance;
        }
        else
        {
            if (!ignoreErrors)
                Debug.LogError("Can't find ui manager for " + transform.name);
        }
    }
}
