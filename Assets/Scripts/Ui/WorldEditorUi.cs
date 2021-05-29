using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlertCreator))]
public class WorldEditorUi : MonoBehaviour
{
    public static WorldEditorUi Instance { get; private set; }

    public EditorPauseMenu PauseMenu;

    private List<UiSelectable> selectables;

    public AlertCreator AlertCreator { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        selectables = new List<UiSelectable>();
        AlertCreator = gameObject.GetComponent<AlertCreator>();
    }

    public void RegisterSelectable(UiSelectable selectable)
    {
        selectables.Add(selectable);
    }

    public void SelectableWasSelected(UiSelectable selectedSelectable)
    {
        foreach (UiSelectable selectable in selectables)
        {
            if (selectable != selectedSelectable)
                selectable.WasDeselected();
        }
    }

    public void OpenPauseMenu()
    {
        PauseMenu.gameObject.SetActive(true);
        PauseMenu.WasShown();
    }

    public void ClosePauseMenu()
    {
        PauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
    }
}
