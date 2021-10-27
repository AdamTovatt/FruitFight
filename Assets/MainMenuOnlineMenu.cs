using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOnlineMenu : MonoBehaviour
{
    public Button JoinButton;
    public Button HostButton;
    public Button BackButton;

    public MainMenuLobbyMenu LobbyMenu;
    public MainMenuJoinGameMenu JoinGameMenu;

    private MainMenuPlayMenu previousMenu;

    void Start()
    {
        JoinButton.onClick.AddListener(Join);
        HostButton.onClick.AddListener(Host);
        BackButton.onClick.AddListener(Back);
    }

    private void Join()
    {
        JoinGameMenu.gameObject.SetActive(true);
        JoinGameMenu.Show(this);
        gameObject.SetActive(false);
    }

    private void Host()
    {
        if(CustomNetworkManager.Instance.isNetworkActive)
        {
            CustomNetworkManager.Instance.StopClient();
            CustomNetworkManager.Instance.StopHost();
        }

        LobbyMenu.gameObject.SetActive(true);
        LobbyMenu.Show(true, CustomNetworkManager.Instance.networkAddress, this);
    }

    private void Back()
    {
        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);
        gameObject.SetActive(false);
    }

    public void Show(MainMenuPlayMenu previousMenu)
    {
        if(previousMenu != null)
        this.previousMenu = previousMenu;

        this.previousMenu.gameObject.SetActive(false);
        JoinButton.Select();
    }
}
