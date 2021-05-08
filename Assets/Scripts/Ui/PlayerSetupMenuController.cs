using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    public int PlayerIndex { get; set; }

    public TextMeshProUGUI titleText;
    public GameObject readyPanel;
    public GameObject menuPanel;
    public Button readyButton;

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
        titleText.SetText(string.Format("Player {0}", (playerIndex + 1).ToString()));
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    public void SetHat(int hat)
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.Instance.SetPlayerHat(PlayerIndex, hat);
        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
    }
}
