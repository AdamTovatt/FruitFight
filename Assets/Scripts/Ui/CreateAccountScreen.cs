using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountScreen : MonoBehaviour
{
    public Button CloseButton;
    public Button CreateAccountButton;

    public Button UsernameInputArea;
    public Button EmailInputArea;
    public Button PasswordInputArea;
    public Button VerifyPasswordInputArea;

    public Button EmailInformationButton;

    public TextMeshProUGUI EmailText;
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI PasswordText;
    public TextMeshProUGUI VerifyPasswordText;

    public UiKeyboardInput Keyboard;

    private LoginScreen parentScreen;

    private string rawPassword;
    private string rawPassword2;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        UsernameInputArea.onClick.AddListener(StartUsernameInput);
        PasswordInputArea.onClick.AddListener(StartPasswordInput);
        VerifyPasswordInputArea.onClick.AddListener(StartVerifyPasswordInput);
        EmailInputArea.onClick.AddListener(StartEmailInput);
        CreateAccountButton.onClick.AddListener(CreateAccountButtonWasPressed);
        EmailInformationButton.onClick.AddListener(ShowEmailInformation);
    }

    public void SelectDefaultButton()
    {
        EmailInputArea.Select();
    }

    public void Show(LoginScreen parentScreen)
    {
        this.parentScreen = parentScreen;
        SelectDefaultButton();
    }

    private void ShowEmailInformation()
    {
        AlertCreator.Instance.CreateNotification("Will only be used in case you forget your password and will not be shared with anyone.", 5);
    }

    private void StartUsernameInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { OnGotUserName(success, text); };
        Keyboard.OpenKeyboard();
    }

    private void StartVerifyPasswordInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { OnGotVerifyPassword(success, text); };
        Keyboard.OpenKeyboard(true);
    }

    private void StartPasswordInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { OnGotPassword(success, text); };
        Keyboard.OpenKeyboard(true);
    }

    private void StartEmailInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { OnGotEmail(success, text); };
        Keyboard.OpenKeyboard();
    }

    private void OnGotEmail(bool success, string text)
    {
        if (success)
            EmailText.text = text;

        UsernameInputArea.Select();
    }

    private void OnGotVerifyPassword(bool success, string text)
    {
        if (success)
        {
            rawPassword2 = text;
            VerifyPasswordText.text = new string('*', text.Length);
        }

        CreateAccountButton.Select();
    }

    private void OnGotPassword(bool success, string text)
    {
        if (success)
        {
            rawPassword = text;
            PasswordText.text = new string('*', text.Length);
        }

        VerifyPasswordInputArea.Select();
    }

    private void OnGotUserName(bool success, string text)
    {
        if (success)
            UsernameText.text = text.ToLower();

        PasswordInputArea.Select();
    }

    private async void CreateAccountButtonWasPressed()
    {
        if(rawPassword != rawPassword2)
        {
            AlertCreator.Instance.CreateNotification("Passwords don't match");
            return;
        }

        bool success = await ApiUserManager.CreateUserAccount(UsernameText.text, EmailText.text, rawPassword);

        if(success)
        {
            AlertCreator.Instance.CreateNotification("Account was created, you are now logged in");
            Close();
        }
        else
        {
            AlertCreator.Instance.CreateNotification("Unknown error when creating account");
        }
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
        parentScreen.gameObject.SetActive(true);
        parentScreen.SelectDefaultButton();
        parentScreen.CloseFromCreateAccountScreen();
    }
}
