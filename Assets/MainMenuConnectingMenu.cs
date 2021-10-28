using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuConnectingMenu : MonoBehaviour
{
    public Button BackButton;
    public TextMeshProUGUI ConnectingText;
    public MainMenuLobbyMenu MainMenuLobbyMenu;

    private MainMenuJoinGameMenu previousMenu;

    void Start()
    {
        BackButton.onClick.AddListener(Back);
    }

    private void Connected(int id)
    {
        UnBindListeners();

        MainMenuLobbyMenu.gameObject.SetActive(true);
        MainMenuLobbyMenu.Show(false, CustomNetworkManager.Instance.networkAddress, previousMenu.PreviousMenu);

        gameObject.SetActive(false);
    }

    public void Show(MainMenuJoinGameMenu previousMenu)
    {
        ConnectingText.text = string.Format("Connecting to {0}...", CustomNetworkManager.Instance.networkAddress);

        if (previousMenu != null)
            this.previousMenu = previousMenu;

        this.previousMenu.gameObject.SetActive(false);

        BackButton.Select();

        BindListeners();

        CustomNetworkManager.IsOnlineSession = true;
        CustomNetworkManager.Instance.StartClient();
    }

    private void BindListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedClient += ClientDisconnected;
        CustomNetworkManager.Instance.OnConnectedClient += Connected;
    }

    private void UnBindListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedClient -= ClientDisconnected;
        CustomNetworkManager.Instance.OnConnectedClient -= Connected;
    }

    private void ClientDisconnected(int id)
    {
        UnBindListeners();

        AlertCreator.Instance.CreateNotification("Error when connecting to " + CustomNetworkManager.Instance.networkAddress);

        Back();
    }

    private void Back()
    {
        UnBindListeners();

        CustomNetworkManager.IsOnlineSession = false;
        CustomNetworkManager.Instance.StopClient();

        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);

        gameObject.SetActive(false);
    }
}
