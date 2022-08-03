using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuJoinGameMenu : MonoBehaviour
{
    public Color PositiveColor;
    public Color NeutralColor;

    public Button ConnectButton;
    public Button BackButton;

    public TMP_InputField HostAddressInputField;

    public MainMenuConnectingMenu ConnectingMenu;

    public MainMenuOnlineMenu PreviousMenu { get; set; }

    private bool usesLobbyName;

    private void Start()
    {
        ConnectButton.onClick.AddListener(Connect);
        BackButton.onClick.AddListener(Back);
        HostAddressInputField.onEndEdit.AddListener(TextEntered);

        HostAddressInputField.text = "localhost";
    }

    private void OnDestroy()
    {
        ConnectButton.onClick.RemoveAllListeners();
        BackButton.onClick.RemoveAllListeners();
        HostAddressInputField.onEndEdit.RemoveAllListeners();
    }

    private void TextEntered(string text)
    {
        if (text.Contains("-"))
            usesLobbyName = true;
        else
            usesLobbyName = false;

        if (text.Length > 5)
        {
            ConnectButton.image.color = PositiveColor;
            ConnectButton.Select();
        }
        else
            ConnectButton.image.color = NeutralColor;
    }

    public void Show(MainMenuOnlineMenu previousMenu)
    {
        if (previousMenu != null)
            PreviousMenu = previousMenu;

        PreviousMenu.gameObject.SetActive(false);

        ConnectButton.Select();
    }

    private void Connect()
    {
        if (!string.IsNullOrEmpty(HostAddressInputField.text))
        {
            if (CustomNetworkManager.Instance.isNetworkActive)
            {
                CustomNetworkManager.Instance.StopClient();
                CustomNetworkManager.Instance.StopServer();
            }

            CustomNetworkManager.Instance.networkAddress = HostAddressInputField.text;

            ConnectingMenu.gameObject.SetActive(true);
            ConnectingMenu.Show(this);
        }
        else
        {
            AlertCreator.Instance.CreateNotification("Invalid host address");
        }
    }

    private void Back()
    {
        PreviousMenu.gameObject.SetActive(true);
        PreviousMenu.Show(null);
        gameObject.SetActive(false);
    }
}
