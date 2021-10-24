using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public EditorLoadLevelScreen LoadLevelScreen;
    public LevelPropertiesScreen LevelPropertiesScreen;

    public List<GameObject> UiSectionPanels;

    public GameObject EventSystem;

    private LoadingScreen loadingScreen;
    private InputSystemUIInputModule uiInput;

    public bool? KeyboardExists { get; set; }

    public AlertCreator AlertCreator { get; set; }
    public UiKeyboardInput OnScreenKeyboard { get; set; }

    private Dictionary<GameObject, bool> uiSectionPanelsOriginalActiveStatus;

    private PlayerControls playerInput;

    private void Awake()
    {
        uiSectionPanelsOriginalActiveStatus = new Dictionary<GameObject, bool>();
        foreach (GameObject uiSectionPanel in UiSectionPanels)
            uiSectionPanelsOriginalActiveStatus.Add(uiSectionPanel, uiSectionPanel.activeSelf);

        if (Instance == null)
        {
            Instance = this;
            eventSystemInstance = EventSystem;
            KeyboardExists = eventSystemInstance.GetComponent<InputSystemUIInputModule>().actionsAsset.controlSchemes.Where(x => x.name == "Keyboard").Count() > 0;

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

        MouseOverSelectableChecker.Enable();

        playerInput = new PlayerControls();
        playerInput.Ui.Select.performed += SelectPerformed;
        playerInput.Ui.Enable();
    }

    private void SelectPerformed(InputAction.CallbackContext context)
    {
        if (BlockMenu.IsOpen)
        {
            MouseOverSelectableChecker.ClickCurrentItem();
        }
    }

    public void DisableAllButOneSectionPanels(GameObject visiblePanel)
    {
        foreach (GameObject uiSectionPanel in UiSectionPanels)
        {
            if (uiSectionPanel != visiblePanel)
            {
                uiSectionPanelsOriginalActiveStatus[uiSectionPanel] = uiSectionPanel.activeSelf;
                uiSectionPanel.SetActive(false);
            }
        }
    }

    public void EnableSectionPanelsAgain(GameObject visiblePanel)
    {
        foreach (GameObject uiSectionPanel in UiSectionPanels)
        {
            if (uiSectionPanel != visiblePanel)
            {
                uiSectionPanel.SetActive(uiSectionPanelsOriginalActiveStatus[uiSectionPanel]);
            }
        }
    }

    private void HandeThumbnailsCreated(object sender, Dictionary<string, Texture2D> thumbnails)
    {
        BlockMenu.gameObject.SetActive(true);
        BlockMenu.ThumbnailsWereCreated();
        HideLoadingScreen();
    }

    public void Destroy()
    {
        MouseOverSelectableChecker.Disable();
        WorldEditor.Instance.ThumbnailManager.OnThumbnailsCreated -= HandeThumbnailsCreated;
        Destroy(EventSystem);
        Destroy(gameObject);
        Destroy(this);
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

        BlockMenu.DisableDeselectButtons();
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

        BlockMenu.EnableDeslectButtons();
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

    public void ShowLevelProperties()
    {
        DisableAllButOneSectionPanels(LevelPropertiesScreen.gameObject);
        LevelPropertiesScreen.gameObject.SetActive(true);
        LevelPropertiesScreen.Show();
    }

    public void CloseLevelProperties()
    {
        EnableSectionPanelsAgain(LevelPropertiesScreen.gameObject);
        LevelPropertiesScreen.gameObject.SetActive(false);
        PauseMenu.LevelPropertiesButton.Select();
    }
}
