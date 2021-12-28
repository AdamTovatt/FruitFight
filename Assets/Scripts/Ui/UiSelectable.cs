using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UiSelectable : MonoBehaviour, ISelectHandler
{
    public GameObject SelectedMarkerPrefab;
    public bool UseCustomContentSize = false;
    public Vector2 ContentSize = new Vector2(300, 50);
    public bool UseCustomPivot = false;
    public Vector2 Pivot = new Vector2(0.5f, 0.5f);
    public bool RequirePointOnSelectable = false; // if the selectable should enforce a point on the selectable to be selected, this is controlled in MouseOverSelectableChecker.cs

    public delegate void SelectedHandler();
    public event SelectedHandler OnSelected;

    private GameObject selectedMarker = null;
    private UiManager manager { get { if (_manager == null) SetupManager(); return _manager; } set { _manager = value; } }
    private UiManager _manager;

    private Toggle toggle;
    private TMP_InputField inputField;
    private Button button;
    private float lastSelectTime;

    private RectTransform rectTransform;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        SetupManager();
    }

    private void Awake()
    {
        SetupManager(true);

        button = gameObject.GetComponent<Button>();

        if (button == null)
            inputField = gameObject.GetComponent<TMP_InputField>();
        if (inputField == null)
            toggle = gameObject.GetComponent<Toggle>();

        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    public bool PointIsOnSelectable(Vector2 point)
    {
        if (point.x < rectTransform.position.x)
            return false;
        if (point.y > rectTransform.position.y)
            return false;
        if (point.x > rectTransform.position.x + rectTransform.sizeDelta.x)
            return false;
        if (point.y < rectTransform.position.y - rectTransform.sizeDelta.y)
            return false;

        return true;
    }

    public void WasDeselected()
    {
        if (selectedMarker != null)
        {
            Destroy(selectedMarker);
            selectedMarker = null;
        }
    }

    public void SelectUnderlyingComponent()
    {
        if (button != null)
        {
            button.Select();
        }
        else if (inputField != null)
        {
            inputField.Select();
        }
        else if(toggle != null)
        {
            toggle.Select();
        }
    }

    public void ForceClick()
    {
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();

        manager.SelectableWasSelected(this);

        if (selectedMarker == null)
        {
            if (SelectedMarkerPrefab != null)
            {
                selectedMarker = Instantiate(SelectedMarkerPrefab, transform.position, transform.rotation, transform);
                selectedMarker.GetComponent<SelectedIndicatorBase>().SetContentSize(UseCustomContentSize ? ContentSize : rect.sizeDelta, UseCustomPivot ? Pivot : rect.pivot);
                AudioManager.Instance.Play("select");
            }

            OnSelected?.Invoke();
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
        else if (GameUi.Instance != null)
        {
            GameUi.Instance.RegisterSelectable(this);
            manager = GameUi.Instance;
        }
        else
        {
            if (!ignoreErrors)
                Debug.LogError("Can't find ui manager for " + transform.name);
        }
    }
}
