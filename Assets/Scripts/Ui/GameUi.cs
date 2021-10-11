using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AlertCreator))]
public class GameUi : UiManager
{
    public GameObject PlayerInfoUiPrefab;
    public Canvas Canvas;
    public InputSystemUIInputModule UiInput;
    public EventSystem EventSystem;
    public InGamePauseMenu PauseMenu;
    public LoadingScreen LoadingScreen;

    public static GameUi Instance { get; private set; }

    private List<UiPlayerInfo> playerInfos;
    private PlayerControls playerInput;

    private void Awake()
    {
        playerInfos = new List<UiPlayerInfo>();
        Instance = this;
        AlertCreator.SetInstance(gameObject.GetComponent<AlertCreator>());
        playerInput = new PlayerControls();
        playerInput.Ui.Select.performed += SelectPerformed;
        playerInput.Ui.Enable();
    }

    private void Start()
    {
        UiInput.enabled = false;

        if (WorldEditorUi.Instance != null) //the world editor has it's own input system that will take over if we come from the editor
        {
            EventSystem.enabled = false;
        }

        PauseMenu.OnExitLevel += ExitLevel;
    }

    private void ExitLevel()
    {
        MouseOverSeletableChecker.Disable();


        if (WorldEditor.IsTestingLevel)
        {
            WorldEditor.Instance.ExitLevelTest();
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
            SceneManager.sceneLoaded += MainMenuWasEntered;
        }
    }

    private void MainMenuWasEntered(Scene scene, LoadSceneMode mode)
    {
        if (MainMenuUi.Instance != null)
            MainMenuUi.Instance.WasReentered();

        SceneManager.sceneLoaded -= MainMenuWasEntered;
    }

    public void CreatePlayerInfoUi(PlayerInformation playerInformation)
    {
        UiPlayerInfo uiPlayerInfo = Instantiate(PlayerInfoUiPrefab, Canvas.transform).GetComponent<UiPlayerInfo>();
        uiPlayerInfo.Init(playerInformation, !(playerInfos.Count > 0)); //if it's the first player we will set it to be left
        playerInfos.Add(uiPlayerInfo);
    }

    private void SelectPerformed(InputAction.CallbackContext context)
    {
        if (context.control.ToString().ToLower().Contains("mouse"))
        {
            MouseOverSeletableChecker.ClickCurrentItem();
        }
    }

    public void ShowPauseMenu()
    {
        UiInput.enabled = true;
        PauseMenu.gameObject.SetActive(true);
        PauseMenu.Show();
        PauseMenu.OnClosed += PauseMenuWasClosed;
        Cursor.lockState = CursorLockMode.None;
        MouseOverSeletableChecker.Enable();
    }

    public void PauseMenuWasClosed()
    {
        if (UiInput == null)
        {
            Destroy(this);
            return;
        }

        UiInput.enabled = false;
        HidePauseMenu();
        GameManager.Instance.GameWasResumed();
        MouseOverSeletableChecker.Disable();
    }

    public void HidePauseMenu()
    {
        PauseMenu.gameObject.SetActive(false);
    }

    public void HideLoadingScreen()
    {
        LoadingScreen.gameObject.SetActive(false);
    }

    public void ShowLoadingScreen()
    {
        LoadingScreen.gameObject.SetActive(true);
    }
}
