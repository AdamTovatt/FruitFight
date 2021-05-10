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
    public GameObject ReadyPanel;
    public GameObject MenuPanel;
    public Button ReadyButton;
    public UiModelDisplay UiModelDisplay;
    public MultipleChoiceSlider HatSlider;

    private float ignoreInputTime = 0.2f;
    private bool inputEnabled;
    private PlayerInput playerInput;

    private string[] hatTexts = new string[] { "No hat", "Wizard Hat", "Beanie" };

    void Update()
    {
        if(Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    private void Start()
    {
        HatSlider.Slider.Select();
        HatSlider.SetText(hatTexts[0]);
        HatSlider.OnValueChanged += (sender, value) => { HatSliderValueChanged(value); };
        HatSlider.Slider.value = HatSlider.Slider.value + 1;
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
        Debug.Log("hat prefab: " +hatPrefab);
        uiBananaMan.SetHat(hatPrefab);

        //ReadyPanel.SetActive(true);
        //ReadyButton.Select();
        //MenuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        ReadyButton.gameObject.SetActive(false);
    }
}
