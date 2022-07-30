using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountScreen : MonoBehaviour
{
    public Color ValidColor;
    public Color NeutralColor;
    public Color ErrorColor;

    public Button CloseButton;
    public Button CreateAccountButton;

    public TMP_InputField UsernameInputArea;
    public TMP_InputField EmailInputArea;
    public TMP_InputField PasswordInputArea;
    public TMP_InputField VerifyPasswordInputArea;

    public Button EmailInformationButton;

    public UiKeyboardInput Keyboard;

    private LoginScreen parentScreen;

    private string rawPassword;
    private string rawPassword2;

    private bool emailValid;
    private bool usernameValid;
    private bool passwordValid;
    private bool password2Valid;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        UsernameInputArea.onEndEdit.AddListener(GotUserName);
        PasswordInputArea.onEndEdit.AddListener(GotPassword);
        VerifyPasswordInputArea.onEndEdit.AddListener(GotVerifyPassword);
        EmailInputArea.onEndEdit.AddListener(GotEmail);
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

    private void GotEmail(string text)
    {
        if (text.Contains("@"))
        {
            emailValid = true;
            EmailInputArea.image.color = ValidColor;
        }
        else
        {
            EmailInputArea.image.color = ErrorColor;
            emailValid = false;
        }

        UpdateCreateButtonColor();
    }

    private void GotPassword(string text)
    {
        rawPassword = text;

        if (string.IsNullOrEmpty(text))
        {
            PasswordInputArea.image.color = ErrorColor;
            passwordValid = false;
        }
        else
        {
            PasswordInputArea.image.color = ValidColor;
            passwordValid = true;
        }

        UpdateCreateButtonColor();
    }

    private void GotVerifyPassword(string text)
    {
        rawPassword2 = text;

        if (string.IsNullOrEmpty(text) || rawPassword2 != rawPassword)
        {
            VerifyPasswordInputArea.image.color = ErrorColor;
            passwordValid = false;
        }
        else
        {
            VerifyPasswordInputArea.image.color = ValidColor;
            password2Valid = true;
        }

        UpdateCreateButtonColor();
    }

    private void GotUserName(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            UsernameInputArea.image.color = ErrorColor;
            usernameValid = false;
        }
        else
        {
            UsernameInputArea.image.color = ValidColor;
            usernameValid = true;
        }

        UpdateCreateButtonColor();
    }

    private void UpdateCreateButtonColor()
    {
        if(emailValid && usernameValid && passwordValid && password2Valid)
        {
            CreateAccountButton.image.color = ValidColor;
        }
        else
        {
            CreateAccountButton.image.color = NeutralColor;
        }
    }

    private async void CreateAccountButtonWasPressed()
    {
        if (rawPassword != rawPassword2)
        {
            AlertCreator.Instance.CreateNotification("Passwords don't match");
            return;
        }

        if(!emailValid)
        {
            AlertCreator.Instance.CreateNotification("Invalid email");
            return;
        }

        if (!usernameValid)
        {
            AlertCreator.Instance.CreateNotification("Invalid username");
            return;
        }

        if (!passwordValid)
        {
            AlertCreator.Instance.CreateNotification("Invalid password");
            return;
        }

        if (!password2Valid)
        {
            AlertCreator.Instance.CreateNotification("Invalid password verification");
            return;
        }

        bool success = await ApiUserManager.CreateUserAccount(UsernameInputArea.text, EmailInputArea.text, rawPassword);

        if (success)
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
        gameObject.SetActive(false);
        parentScreen.gameObject.SetActive(true);
        parentScreen.SelectDefaultButton();
        parentScreen.CloseFromCreateAccountScreen();
    }
}
