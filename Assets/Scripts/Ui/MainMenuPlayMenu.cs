using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPlayMenu : MonoBehaviour
{
    public Button HelpButton;
    public Button LocalMultiplayerButton;
    public Button OnlineMultiplayerButton;
    public Button BackButton;

    public MainMenuStart PreviousMenu;
    public MainMenuLocalMenu LocalPlayMenu;
    public MainMenuOnlineMenu OnlinePlayMenu;

    void Start()
    {
        BackButton.onClick.AddListener(Back);
        LocalMultiplayerButton.onClick.AddListener(LocalPlay);
        HelpButton.onClick.AddListener(Help);
        OnlineMultiplayerButton.onClick.AddListener(OnlinePlay);
    }

    public void Show(MainMenuStart previousMenu)
    {
        LocalMultiplayerButton.Select();

        if (previousMenu != null)
            PreviousMenu = previousMenu;

        PreviousMenu.gameObject.SetActive(false);
    }

    private void Help()
    {
        AlertCreator.Instance.CreateNotification("Online = play online multiplayer with friends", 5);
        AlertCreator.Instance.CreateNotification("Local = single player or split screen multiplayer", 5);
    }

    private void LocalPlay()
    {
        LocalPlayMenu.gameObject.SetActive(true);
        LocalPlayMenu.Show(this);
        gameObject.SetActive(false);
    }

    private void OnlinePlay()
    {
        OnlinePlayMenu.gameObject.SetActive(true);
        OnlinePlayMenu.Show(this);
        gameObject.SetActive(false);
    }

    private void Back()
    {
        PreviousMenu.gameObject.SetActive(true);
        PreviousMenu.Show();
        gameObject.SetActive(false);
    }
}
