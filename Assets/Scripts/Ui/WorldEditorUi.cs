using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(AlertCreator))]
public class WorldEditorUi : MonoBehaviour
{
    public static WorldEditorUi Instance { get; private set; }
    private static GameObject eventSystemInstance;

    public EditorPauseMenu PauseMenu;
    public EditorBlockMenu BlockMenu;
    public TestLevelPauseMenu TestLevelPauseMenu;

    public GameObject EventSystem;

    private List<UiSelectable> selectables;
    private LoadingScreen loadingScreen;
    private InputSystemUIInputModule uiInput;

    public AlertCreator AlertCreator { get; set; }
    public UiKeyboardInput UiKeyboardInput { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            eventSystemInstance = EventSystem;
        }
        else
        {
            Destroy();
            return;
        }

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystem);

        selectables = new List<UiSelectable>();
        AlertCreator = gameObject.GetComponent<AlertCreator>();
        UiKeyboardInput = gameObject.GetComponent<UiKeyboardInput>();
        loadingScreen = gameObject.GetComponentInChildren<LoadingScreen>();
    }

    private void Start()
    {
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated += HandeThumbnailsCreated;

        uiInput = EventSystem.GetComponent<InputSystemUIInputModule>();
    }

    private void HandeThumbnailsCreated(object sender, Dictionary<string, Texture2D> thumbnails)
    {
        BlockMenu.gameObject.SetActive(true);
        BlockMenu.ThumbnailsWereCreated();
        HideLoadingScreen();
    }

    public void Destroy()
    {
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated -= HandeThumbnailsCreated;
        Destroy(EventSystem);
        Destroy(gameObject);
        Destroy(this);
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
        if (uiInput != null)
            uiInput.enabled = true;

        GameManager.Instance.DisablePlayerControls();
        TestLevelPauseMenu.gameObject.SetActive(true);
        TestLevelPauseMenu.WasShown();
    }

    public void CloseLevelTestPauseMenu()
    {
        if (uiInput != null)
            uiInput.enabled = false;

        GameManager.Instance.EnablePlayerControls();
        TestLevelPauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
    }

    public void OpenPauseMenu()
    {
        if (uiInput != null)
            uiInput.enabled = true;

        UiKeyboardInput.OpenKeyboard();
        //PauseMenu.gameObject.SetActive(true);
        //PauseMenu.WasShown();
    }

    public void ClosePauseMenu()
    {
        if (uiInput != null)
            uiInput.enabled = false;

        PauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
    }

    public void HideBlockSelection()
    {
        if (uiInput != null)
            uiInput.enabled = false;

        BlockMenu.gameObject.SetActive(false);
    }

    public void ShowBlockSelection()
    {
        if (uiInput != null)
            uiInput.enabled = true;

        BlockMenu.gameObject.SetActive(true);
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.Show();
    }

    public void HideLoadingScreen()
    {
        if (uiInput != null)
            uiInput.enabled = false;

        loadingScreen.Hide();
    }
}
