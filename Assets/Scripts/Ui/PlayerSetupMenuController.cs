using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    public int PlayerIndex { get; set; }

    public TextMeshProUGUI TitleText;
    public GameObject ReadyPanel;
    public GameObject MenuPanel;
    public Button ReadyButton;
    public UiModelDisplay UiModelDisplay;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;

    void Update()
    {
        if(Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SetPlayerIndex(int playerIndex)
    {
        PlayerIndex = playerIndex;
        TitleText.SetText(string.Format("Player {0}", (playerIndex + 1).ToString()));
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    public void SetHat(int hat)
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.Instance.SetPlayerHat(PlayerIndex, hat);

        UiBananaMan uiBananaMan = UiModelDisplay.Model.GetComponent<UiBananaMan>();
        uiBananaMan.SetHat((Prefab)(hat-1));

        ReadyPanel.SetActive(true);
        ReadyButton.Select();
        MenuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        ReadyButton.gameObject.SetActive(false);
    }
}
