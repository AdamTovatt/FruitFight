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
    public UiModelDisplay UiModelDisplay;
    public MultipleChoiceSlider HatSlider;

    private float ignoreInputTime = 0.2f;
    private bool inputEnabled;
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private string[] hatTexts = new string[] { "<- No hat ->", "<- Wizard Hat ->", "<- Beanie ->" };

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
        ReadyInstructionsText.text = string.Format("Press {0} to ready up", GetButtonName(playerControls.Ui.Select));
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

    private string GetButtonName(InputAction inputAction)
    {
        return inputAction.name;
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
            return;

        if(ReadyText != null) ReadyText.gameObject.SetActive(true);
        ReadyInstructionsText.text = string.Format("Press {0} to unready", GetButtonName(playerControls.Ui.Cancel));
        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
    }

    public void UnReadyPlayer()
    {
        if (!inputEnabled)
            return;

        ReadyText.gameObject.SetActive(false);
        ReadyInstructionsText.text = string.Format("Press {0} to ready up", GetButtonName(playerControls.Ui.Select));
        PlayerConfigurationManager.Instance.UnReadyPlayer(PlayerIndex);
    }
}
