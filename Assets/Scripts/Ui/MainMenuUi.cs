using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AlertCreator))]
public class MainMenuUi : UiManager
{
    public static MainMenuUi Instance;

    public LoadingScreen LoadingScreen;
    public MainMenuStart StartMenu;
    public BrowseLevelsScreen BrowseLevelsScreen;

    public EventSystem EventSystem;

    public bool KeyboardExists { get; set; }

    private InputSystemUIInputModule uiInput;

    private bool cameBackFromMultiplayer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        KeyboardExists = EventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset.controlSchemes.Where(x => x.name == "Keyboard").Count() > 0;
        uiInput = EventSystem.GetComponent<InputSystemUIInputModule>();

        AlertCreator.SetInstance(gameObject.GetComponent<AlertCreator>());
    }

    private void Start()
    {
        MouseOverSelectableChecker.Enable();

        if (!cameBackFromMultiplayer)
            StartCoroutine(ShowMenuWithDelay());

        ApiHelper.PingServer();
    }

    public void WasReentered()
    {
        gameObject.SetActive(true);
        MouseOverSelectableChecker.Enable();

        if (EventSystem == null)
            EventSystem = FindObjectOfType<EventSystem>();

        if (PlayerConfigurationManager.Instance != null)
            Destroy(PlayerConfigurationManager.Instance.gameObject);

        MouseOverSelectableChecker.eventSystem = EventSystem;

        uiInput = EventSystem.GetComponent<InputSystemUIInputModule>();
        uiInput.enabled = false;
        uiInput.enabled = true;

        if (BrowseLevelsScreen.LevelDetailsScreen.gameObject.activeSelf)
        {
            Debug.Log("It's active");
            BrowseLevelsScreen.LevelDetailsScreen.SelectDefaultButton();
        }

        gameObject.transform.parent = null;

        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.isNetworkActive)
            {
                Debug.Log("This is an online session and we got back from a level");
                cameBackFromMultiplayer = true;
                StartMenu.gameObject.SetActive(false);
                LoadingScreen.Hide();
                StartMenu.PlayMenu.OnlinePlayMenu.LobbyMenu.gameObject.SetActive(true);
                StartMenu.PlayMenu.OnlinePlayMenu.LobbyMenu.Show(false, CustomNetworkManager.Instance.networkAddress, StartMenu.PlayMenu.OnlinePlayMenu);
                StartMenu.PlayMenu.OnlinePlayMenu.LobbyMenu.FindExistingPlayerIdentities();
            }
        }
    }

    private IEnumerator ShowMenuWithDelay()
    {
        yield return new WaitForSeconds(0.2f);

        if (!cameBackFromMultiplayer)
        {
            StartMenu.gameObject.SetActive(true);
            LoadingScreen.Hide();
            StartMenu.PlayButton.Select();
        }
    }

    public void LevelEditorButtonWasPressed()
    {
        cameBackFromMultiplayer = false;

        MouseOverSelectableChecker.Disable();
        LoadingScreen.Show();

        Destroy(gameObject);

        SceneManager.LoadScene("LevelEditor");
        Instance = null;
    }

    public void PlayButtonWasPressed()
    {
        cameBackFromMultiplayer = false;

        MouseOverSelectableChecker.Disable();
        LoadingScreen.Show();
        SceneManager.LoadScene("GamePlay");
        Instance = null;
    }

    public void BrowseLevelsButtonWasPressed()
    {
        cameBackFromMultiplayer = false;

        StartMenu.gameObject.SetActive(false);
        BrowseLevelsScreen.gameObject.SetActive(true);
        BrowseLevelsScreen.Show();
    }

    public void ExitBrowseLevelsScreen()
    {
        cameBackFromMultiplayer = false;

        StartMenu.gameObject.SetActive(true);
        BrowseLevelsScreen.gameObject.SetActive(false);
        StartMenu.BrowseLevelsButton.Select();
    }

    public void DisableUiInput()
    {
        if (uiInput != null)
            uiInput.enabled = false;
    }

    public void EnableUiInput()
    {
        if (uiInput != null)
            uiInput.enabled = true;
    }
}
