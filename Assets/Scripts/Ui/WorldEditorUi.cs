using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlertCreator))]
public class WorldEditorUi : MonoBehaviour
{
    public static WorldEditorUi Instance { get; private set; }

    public EditorPauseMenu PauseMenu;
    public EditorBlockMenu BlockMenu;

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

    private void Start()
    {
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated += (sender, thumbnails) =>
        {
            BlockMenu.gameObject.SetActive(true);
            BlockMenu.SetSize(200, 200);
            gameObject.GetComponentInChildren<LoadingScreen>().Hide();
        };
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
