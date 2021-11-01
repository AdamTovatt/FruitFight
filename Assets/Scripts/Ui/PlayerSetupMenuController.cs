using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSetupMenuController : NetworkBehaviour
{
    public int PlayerIndex { get { return playerIndex; } }

    [SyncVar]
    private int playerIndex;

    public TextMeshProUGUI TitleText;
    public GameObject ReadyText;
    public TextMeshProUGUI ReadyInstructionsText;
    public Image ReadyInstructionsIcon;
    public UiModelDisplay UiModelDisplay;
    public MultipleChoiceSlider HatSlider;

    private float ignoreInputTime = 0.2f;
    private bool inputEnabled;
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private string[] hatTexts = new string[] { "<- No hat ->", "<- Wizard Hat ->", "<- Beanie ->", "<- Sweat Band ->", "<- Mushroom Hat ->" };

    public bool LocalPlayer { get; set; }

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        GameObject rootMenu = GameObject.Find("MainLayout");
        gameObject.transform.SetParent(rootMenu.transform);
    }

    private void Start()
    {
        UiModelDisplay.GetComponent<RawImage>().enabled = true;

        if (LocalPlayer)
        {
            Debug.Log("bind local player hat change");
            HatSlider.InitializeInput(playerInput);
            HatSlider.SetText(hatTexts[0]);
            HatSlider.OnValueChanged += (sender, value) => { HatSliderValueChanged(value); };

            playerInput.onActionTriggered += HandleInput;
        }

        SetReadyInstructionsText(false);
    }

    private void SetReadyInstructionsText(bool playerIsReady)
    {
        Device device = Device.Unspecified;

        if (playerInput != null)
        {
            if (playerInput.currentControlScheme == "Keyboard")
                device = Device.Keyboard;
            else if (playerInput.currentControlScheme == "Gamepad")
                device = Device.Gamepad;
        }

        if (!playerIsReady)
        {
            ReadyInstructionsIcon.sprite = IconConfiguration.Get().GetIcon("Select").GetSpriteByDevice(device);
            ReadyInstructionsText.text = "to ready up";
        }
        else
        {
            ReadyInstructionsIcon.sprite = IconConfiguration.Get().GetIcon("Cancel").GetSpriteByDevice(device);
            ReadyInstructionsText.text = "to unready";
        }
    }

    private void UnbindEvents()
    {
        if (playerInput != null)
            playerInput.onActionTriggered -= HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (context.action.id == playerControls.Ui.Select.id)
        {
            HatSlider.InputEnabled = false;
            ReadyPlayer();
        }
        else if (context.action.id == playerControls.Ui.Cancel.id)
        {
            HatSlider.InputEnabled = true;
            UnReadyPlayer();
        }
    }

    public void SetPlayerIndex(PlayerInput input, bool localPlayer)
    {
        if (input != null)
            playerIndex = input.playerIndex;

        LocalPlayer = true;
        playerInput = input;
        TitleText.SetText(string.Format("Player {0}", (PlayerIndex + 1).ToString()));
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    [ClientRpc]
    public void AquireControl()
    {
        if (!CustomNetworkManager.Instance.IsServer)
        {
            PlayerSetupMenuController[] controllers = FindObjectsOfType<PlayerSetupMenuController>();
            Debug.Log("con: " + controllers.Length);
            Debug.Log("loc: " + controllers.Where(x => x.LocalPlayer).Count());

            PlayerSetupMenuController old = controllers.Where(x => x.LocalPlayer).FirstOrDefault();
            if (old != null)
            {
                SetPlayerIndex(old.playerInput, true);
                old.UnbindEvents();
                Destroy(old.gameObject);
                Debug.Log("destroyed old");
            }
            else
            {
                Debug.LogError("Could not find player setup menu controller for local player to aquire control over");
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void ClientChangedHat(int newHat)
    {
        HatSliderValueChanged(newHat);
    }

    [ClientRpc]
    private void ServerChangedHat(int newHat)
    {
        HatSliderValueChanged(newHat, true);
    }

    private void HatSliderValueChanged(int value, bool isFromRpc = false)
    {
        if (!CustomNetworkManager.IsOnlineSession || isFromRpc)
        {
            HatSlider.SetText(hatTexts[value]);
            SetHat(value);
        }
        else
        {
            if (!CustomNetworkManager.Instance.IsServer)
            {
                ClientChangedHat(value);
            }
            else
            {
                ServerChangedHat(value);
            }
        }
    }

    public void SetHat(int hat)
    {
        if (!CustomNetworkManager.IsOnlineSession)
            PlayerConfigurationManager.Instance.SetPlayerHat(PlayerIndex, hat);
        else
            Debug.Log("we should set the hat in the playernetworkidentity maybe");

        UiBananaMan uiBananaMan = UiModelDisplay.Model.GetComponent<UiBananaMan>();

        Prefab? hatPrefab = null;
        if (hat > 0)
            hatPrefab = (Prefab)(hat - 1);
        uiBananaMan.SetHat(hatPrefab);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
            return;

        if (ReadyText != null)
            ReadyText.gameObject.SetActive(true);

        SetReadyInstructionsText(true);

        UiModelDisplay.OnImageGenerated += (Texture2D image) => { PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex, image); };
        if (this != null)
            StartCoroutine(UiModelDisplay.GenerateImage());
    }

    public void UnReadyPlayer()
    {
        if (!inputEnabled)
            return;

        ReadyText.gameObject.SetActive(false);
        SetReadyInstructionsText(false);
        PlayerConfigurationManager.Instance.UnReadyPlayer(PlayerIndex);
    }
}
