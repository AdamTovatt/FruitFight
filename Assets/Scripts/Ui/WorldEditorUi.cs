using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(AlertCreator))]
public class WorldEditorUi : MonoBehaviour
{
    public static WorldEditorUi Instance { get; private set; }

    public EditorPauseMenu PauseMenu;
    public EditorBlockMenu BlockMenu;
    public TestLevelPauseMenu TestLevelPauseMenu;

    public GameObject EventSystem;

    private List<UiSelectable> selectables;
    private LoadingScreen loadingScreen;
    private InputSystemUIInputModule uiInput;

    public AlertCreator AlertCreator { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystem);

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

        uiInput = EventSystem.GetComponent<InputSystemUIInputModule>();
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
        uiInput.enabled = true;
        GameManager.Instance.DisablePlayerControls();
        TestLevelPauseMenu.gameObject.SetActive(true);
        TestLevelPauseMenu.WasShown();
    }

    public void CloseLevelTestPauseMenu()
    {
        uiInput.enabled = false;
        GameManager.Instance.EnablePlayerControls();
        TestLevelPauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
    }

    public void OpenPauseMenu()
    {
        uiInput.enabled = true;
        PauseMenu.gameObject.SetActive(true);
        PauseMenu.WasShown();
    }

    public void ClosePauseMenu()
    {
        uiInput.enabled = false;
        PauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
    }

    public void HideBlockSelection()
    {
        uiInput.enabled = false;
        BlockMenu.gameObject.SetActive(false);
    }

    public void ShowBlockSelection()
    {
        uiInput.enabled = true;
        BlockMenu.gameObject.SetActive(true);
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.Show();
    }

    public void HideLoadingScreen()
    {
        uiInput.enabled = false;
        loadingScreen.Hide();
    }
}
