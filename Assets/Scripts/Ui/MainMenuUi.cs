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

    private void Awake()
    {
        if(Instance != null)
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

        if(BrowseLevelsScreen.LevelDetailsScreen.gameObject.activeSelf)
        {
            Debug.Log("It's active");
            BrowseLevelsScreen.LevelDetailsScreen.SelectDefaultButton();
        }

        gameObject.transform.parent = null;
    }

    private IEnumerator ShowMenuWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        StartMenu.gameObject.SetActive(true);
        LoadingScreen.Hide();
        StartMenu.PlayButton.Select();
    }

    public void LevelEditorButtonWasPressed()
    {
        MouseOverSelectableChecker.Disable();
        LoadingScreen.Show();

        Destroy(gameObject);

        SceneManager.LoadScene("LevelEditor");
        Instance = null;
    }

    public void PlayButtonWasPressed()
    {
        MouseOverSelectableChecker.Disable();
        LoadingScreen.Show();
        SceneManager.LoadScene("GamePlay");
        Instance = null;
    }

    public void BrowseLevelsButtonWasPressed()
    {
        StartMenu.gameObject.SetActive(false);
        BrowseLevelsScreen.gameObject.SetActive(true);
        BrowseLevelsScreen.Show();
    }

    public void ExitBrowseLevelsScreen()
    {
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
