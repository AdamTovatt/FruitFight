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
    public EditorLoadLevelScreen LoadLevelScreen;
    public LevelPropertiesScreen LevelPropertiesScreen;
    public BehaviourMenu2 BehaviourMenu2;

    public List<GameObject> UiSectionPanels;

    public GameObject EventSystem;

    private LoadingScreen loadingScreen;
    private InputSystemUIInputModule uiInput;

    public bool PauseMenuIsOpen { get; private set; }

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

    public void OpenBehaviourMenu()
    {
        EnableUiInput();

        OpenPanel(BehaviourMenu2);
    }

    public void CloseBehaviourMenu()
    {
        DisableUiInput();

        ClosePanel(BehaviourMenu2);
    }

    public void OpenPanel(PanelBase panel)
    {
        panel.gameObject.SetActive(true);
        if (panel.Show())
        {
            BlockMenu.gameObject.SetActive(false);
        }
        else
        {
            panel.gameObject.SetActive(false);
            Debug.LogError("Error when opening panel");
        }
    }

    public void ClosePanel(PanelBase panel)
    {
        if (panel.Hide())
        {
            panel.gameObject.SetActive(false);
            BlockMenu.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Error when closing panel");
        }
    }

    public void EscapeWasPressed()
    {
        if (UiSectionPanels.Count(x => x.gameObject.activeSelf && x.transform.name != "BlockMenu") == 0)
        {
            OpenPauseMenu();
        }
        else
        {
            if (PauseMenuIsOpen)
                ClosePauseMenu();

            foreach (GameObject panel in UiSectionPanels)
            {
                if (panel.transform.name == "BlockMenu")
                {
                    Debug.Log("Block menu");
                    panel.SetActive(true); //this is the "default" panel
                }
                else
                {
                    PanelBase panelBase = gameObject.GetComponent<PanelBase>();
                    if (panelBase != null)
                        ClosePanel(panelBase);
                    else
                        panel.SetActive(false);
                }
            }

            WorldEditor.Instance.EnableControls();
        }
    }

    public void EnterUiControlMode()
    {
        WorldEditor.Instance.DisableControls();
        EnableUiInput();
    }

    public void ExitUiControlMode()
    {
        WorldEditor.Instance.EnableControls();
        DisableUiInput();
    }

    public void OpenPauseMenu()
    {
        EnableUiInput();
        WorldEditor.Instance.DisableControls();

        PauseMenu.gameObject.SetActive(true);
        PauseMenu.WasShown();

        BlockMenu.DisableDeselectButtons();

        PauseMenuIsOpen = true;
        BlockMenu.Close();
    }

    public void ClosePauseMenu()
    {
        DisableUiInput();
        WorldEditor.Instance.EnableControls();

        PauseMenu.gameObject.SetActive(false);
        WorldEditor.Instance.EnableControls();

        BlockMenu.EnableDeslectButtons();
        PauseMenuIsOpen = false;
    }

    public void HideBlockSelection()
    {
        DisableUiInput();

        BlockMenu.gameObject.SetActive(false);
    }

    public void ShowBlockSelection()
    {
        EnableUiInput();

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
