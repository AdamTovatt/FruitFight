using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public Button UsernameInputArea;
    public Button PasswordInputArea;
    public Button LoginButton;
    public Button CreateNewAccountButton;
    public Button CloseButton;

    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI PasswordText;

    public UiKeyboardInput Keyboard;

    public delegate void LoginScreenWasExited();
    public event LoginScreenWasExited OnLoginScreenWasExited;

    private MonoBehaviour previousScreen;

    private void Start()
    {
        UsernameInputArea.onClick.AddListener(StartUsernameInput);
        PasswordInputArea.onClick.AddListener(StartPasswordInput);
        LoginButton.onClick.AddListener(LoginButtonPressed);
        CreateNewAccountButton.onClick.AddListener(CreateNewAccountButtonPressed);
        CloseButton.onClick.AddListener(Close);
    }

    public void Show(MonoBehaviour previousScreen)
    {
        this.previousScreen = previousScreen;
        previousScreen.gameObject.SetActive(false);
        LoginButton.Select();
    }

    private void StartUsernameInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { if (success) UsernameText.text = text; };
        Keyboard.OpenKeyboard();
    }

    private void StartPasswordInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { if (success) PasswordText.text = text; };
        Keyboard.OpenKeyboard();
    }

    private void LoginButtonPressed()
    {
        Login(UsernameText.text, PasswordText.text);

        Close();
    }

    private void Login(string username, string password)
    {
        
    }

    private void Close()
    {
        OnLoginScreenWasExited?.Invoke();
        previousScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void CreateNewAccountButtonPressed()
    {

    }
}
