using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlertCreator))]
public class WorldEditorUi : MonoBehaviour
{
    public static WorldEditorUi Instance { get; private set; }

    public EditorPauseMenu PauseMenu;
    public EditorBlockMenu BlockMenu;
    public TestLevelPauseMenu TestLevelPauseMenu;

    private List<UiSelectable> selectables;
    private LoadingScreen loadingScreen;

    public AlertCreator AlertCreator { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        selectables = new List<UiSelectable>();
        AlertCreator = gameObject.GetComponent<AlertCreator>();
        loadingScreen = gameObject.GetComponentInChildren<LoadingScreen>();
    }

    private void Start()
    {
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated += (sender, thumbnails) =>
        {
            BlockMenu.gameObject.SetActive(true);
            BlockMenu.ThumbnailsWereCreated();
            HideLoadingScreen();
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

    public void OpenLevelTestPauseMenu()
    {
        GameManager.Instance.DisablePlayerControls();
        TestLevelPauseMenu.gameObject.SetActive(true);
        TestLevelPauseMenu.WasShown();
    }

    public void CloseLevelTestPauseMenu()
    {
        GameManager.Instance.EnablePlayerControls();
        TestLevelPauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
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

    public void HideBlockSelection()
    {
        BlockMenu.gameObject.SetActive(false);
    }

    public void ShowBlockSelection()
    {
        BlockMenu.gameObject.SetActive(true);
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.Show();
    }

    public void HideLoadingScreen()
    {
        loadingScreen.Hide();
    }
}
