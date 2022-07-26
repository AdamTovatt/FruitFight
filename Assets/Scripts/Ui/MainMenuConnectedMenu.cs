using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuConnectedMenu : MonoBehaviour
{
    public Button StoryModeButton;
    public Button BrowseLevelsButton;
    public Button BackButton;

    public MainMenuLobbyMenu LobbyMenu;

    void Start()
    {
        StoryModeButton.onClick.AddListener(StoryMode);
        BrowseLevelsButton.onClick.AddListener(BrowseLevels);
        BackButton.onClick.AddListener(Back);
    }

    private void StoryMode()
    {
        Debug.Log("Story mode");
        GameStateManager.SetGameState(GameState.Story);

        MainMenuUi.Instance.StartMenu.PlayMenu.LocalPlayMenu.StoryModeScreen.gameObject.SetActive(true);
        MainMenuUi.Instance.StartMenu.PlayMenu.LocalPlayMenu.StoryModeScreen.Show();
        gameObject.SetActive(false);

        UnbindEventListeners();
    }

    private void BrowseLevels()
    {       
        MainMenuUi.Instance.BrowseLevelsButtonWasPressed();
    }

    private void BindEventListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedServer += ClientLeftFromServer;
    }

    private void UnbindEventListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedServer -= ClientLeftFromServer;
    }

    private void ClientLeftFromServer(int clientId)
    {
        Back();
    }

    private void Back()
    {
        LobbyMenu.gameObject.SetActive(true);
        LobbyMenu.Show();
        UnbindEventListeners();
        gameObject.SetActive(false);
    }

    public void Show(MainMenuLobbyMenu lobbyMenu)
    {
        if (lobbyMenu != null)
            LobbyMenu = lobbyMenu;

        LobbyMenu.gameObject.SetActive(false);
        StoryModeButton.Select();
        BindEventListeners();
    }
}
