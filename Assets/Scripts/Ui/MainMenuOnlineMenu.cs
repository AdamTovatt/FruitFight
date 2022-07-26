using kcp2k;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOnlineMenu : MonoBehaviour
{
    public Button JoinButton;
    public Button HostButton;
    public Button BackButton;

    public LoginScreen LoginScreen;
    public MainMenuLobbyMenu LobbyMenu;
    public MainMenuJoinGameMenu JoinGameMenu;

    private MainMenuPlayMenu previousMenu;

    private bool wasJoining = false;
    private bool wasHosting = false;

    void Start()
    {
        JoinButton.onClick.AddListener(Join);
        HostButton.onClick.AddListener(Host);
        BackButton.onClick.AddListener(Back);
    }

    private void LoginScreenWasClosed()
    {
        if (ApiHelper.UserCredentials != null)
        {
            AlertCreator.Instance.CreateNotification("Login successful");
        }
        else
            AlertCreator.Instance.CreateNotification("You have not been logged in");
    }

    private void RedirectToLogin()
    {
        LoginScreen.OnLoginScreenWasExited += LoginScreenWasClosed;
        LoginScreen.gameObject.SetActive(true);
        LoginScreen.Show(this);
    }

    private void Join()
    {
        wasJoining = false;
        wasHosting = false;

        if (ApiHelper.UserCredentials == null)
        {
            wasJoining = true;
            RedirectToLogin();
            return;
        }

        if (CustomNetworkManager.Instance.isNetworkActive)
        {
            CustomNetworkManager.Instance.StopClient();
            CustomNetworkManager.Instance.StopHost();
        }

        JoinGameMenu.gameObject.SetActive(true);
        JoinGameMenu.Show(this);
        gameObject.SetActive(false);
    }

    private void Host()
    {
        wasJoining = false;
        wasHosting = false;

        if (ApiHelper.UserCredentials == null)
        {
            wasHosting = true;
            RedirectToLogin();
            return;
        }

        if (CustomNetworkManager.Instance.isNetworkActive)
        {
            CustomNetworkManager.Instance.StopClient();
            CustomNetworkManager.Instance.StopHost();
        }

        LobbyMenu.gameObject.SetActive(true);
        LobbyMenu.Show(true, string.Format("Host port: {0}", CustomNetworkManager.Instance.GetComponent<KcpTransport>().Port), this);
    }

    private void Back()
    {
        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);
        gameObject.SetActive(false);
    }

    public void Show(MainMenuPlayMenu previousMenu)
    {
        if (previousMenu != null)
            this.previousMenu = previousMenu;

        if (this.previousMenu == null)
            this.previousMenu = MainMenuUi.Instance.StartMenu.PlayMenu;

        this.previousMenu.gameObject.SetActive(false);
        JoinButton.Select();
    }
}
