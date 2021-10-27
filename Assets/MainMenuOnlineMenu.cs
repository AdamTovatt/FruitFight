using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOnlineMenu : MonoBehaviour
{
    public Button JoinButton;
    public Button HostButton;
    public Button BackButton;

    private MainMenuPlayMenu previousMenu;

    void Start()
    {
        JoinButton.onClick.AddListener(Join);
        HostButton.onClick.AddListener(Host);
        BackButton.onClick.AddListener(Back);
    }

    private void Join()
    {
        Debug.Log("Join player");
    }

    private void Host()
    {
        Debug.Log("Host game");
    }

    private void Back()
    {
        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);
        gameObject.SetActive(false);
    }

    public void Show(MainMenuPlayMenu previousMenu)
    {
        this.previousMenu = previousMenu;
        previousMenu.gameObject.SetActive(false);
        JoinButton.Select();
    }
}
