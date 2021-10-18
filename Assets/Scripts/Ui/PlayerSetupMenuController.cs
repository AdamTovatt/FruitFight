using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    public int PlayerIndex { get { return playerInput.playerIndex; } }

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
    }

    private void Start()
    {
        UiModelDisplay.GetComponent<RawImage>().enabled = true;
        HatSlider.InitializeInput(playerInput);
        HatSlider.SetText(hatTexts[0]);
        HatSlider.OnValueChanged += (sender, value) => { HatSliderValueChanged(value); };

        playerInput.onActionTriggered += HandleInput;
        SetReadyInstructionsText(false);
    }

    private void SetReadyInstructionsText(bool playerIsReady)
    {
        Device device = Device.Unspecified;
        if (playerInput.currentControlScheme == "Keyboard")
            device = Device.Keyboard;
        else if (playerInput.currentControlScheme == "Gamepad")
            device = Device.Gamepad;

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

    public void SetPlayerIndex(PlayerInput input)
    {
        playerInput = input;
        TitleText.SetText(string.Format("Player {0}", (PlayerIndex + 1).ToString()));
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    private void HatSliderValueChanged(int value)
    {
        HatSlider.SetText(hatTexts[value]);
        SetHat(value);
    }

    public void SetHat(int hat)
    {
        PlayerConfigurationManager.Instance.SetPlayerHat(PlayerIndex, hat);
        
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
