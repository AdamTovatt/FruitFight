using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(AlertCreator))]
public class WorldEditorUi : UiManager
{
    public static WorldEditorUi Instance { get; private set; }
    private static GameObject eventSystemInstance;

    public EditorPauseMenu PauseMenu;
    public EditorBlockMenu BlockMenu;
    public TestLevelPauseMenu TestLevelPauseMenu;
    public BehaviourMenu BehaviourMenu;

    public GameObject EventSystem;

    private LoadingScreen loadingScreen;
    private InputSystemUIInputModule uiInput;

    public AlertCreator AlertCreator { get; set; }
    public UiKeyboardInput OnScreenKeyboard { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            eventSystemInstance = EventSystem;

            AlertCreator.SetInstance(gameObject.GetComponent<AlertCreator>()); //set alert creator instance
        }
        else
        {
            AlertCreator.SetInstance(Instance.GetComponent<AlertCreator>()); //set alert creator instance

            Destroy();
            return;
        }

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystem);

        AlertCreator = gameObject.GetComponent<AlertCreator>();
        OnScreenKeyboard = gameObject.GetComponent<UiKeyboardInput>();
        loadingScreen = gameObject.GetComponentInChildren<LoadingScreen>();
    }

    private void Start()
    {
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated += HandeThumbnailsCreated;

        uiInput = EventSystem.GetComponent<InputSystemUIInputModule>();

        MouseOverSeletableChecker.Enable();
    }

    private void HandeThumbnailsCreated(object sender, Dictionary<string, Texture2D> thumbnails)
    {
        BlockMenu.gameObject.SetActive(true);
        BlockMenu.ThumbnailsWereCreated();
        HideLoadingScreen();
    }

    public void Destroy()
    {
        MouseOverSeletableChecker.Disable();
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated -= HandeThumbnailsCreated;
        Destroy(EventSystem);
        Destroy(gameObject);
        Destroy(this);
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

    public void OpenBehaviourMenu(Block block)
    {
        if (uiInput != null)
            uiInput.enabled = true;

        BehaviourMenu.gameObject.SetActive(true);
        BehaviourMenu.Show(block);
    }

    public void CloseBehaviourMenu()
    {
        if (uiInput != null)
            uiInput.enabled = false;

        BehaviourMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();
    }

    public void OpenPauseMenu()
    {
        if (uiInput != null)
            uiInput.enabled = true;

        PauseMenu.gameObject.SetActive(true);
        PauseMenu.WasShown();
    }

    public void ClosePauseMenu()
    {
        if (uiInput != null)
            uiInput.enabled = false;

        if (BehaviourMenu.gameObject.activeSelf)
        {
            BehaviourMenu.Hide();
            BehaviourMenu.gameObject.SetActive(false);
        }

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

    public void EnableUiInput()
    {
        if (uiInput != null)
            uiInput.enabled = true;
    }

    public void DisableUiInput()
    {
        if (uiInput != null)
            uiInput.enabled = false;
    }
}
