using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private string rawPasswordInput;

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
        Keyboard.OnGotText += (sender, success, text) => { OnGotUserName(success, text); };
        Keyboard.OpenKeyboard();
    }

    private void StartPasswordInput()
    {
        Keyboard.OnGotText += (sender, success, text) => { OnGotPassword(success, text); };
        Keyboard.OpenKeyboard(true);
    }

    private void OnGotUserName(bool success, string text)
    {
        if (success) 
            UsernameText.text = text;

        PasswordInputArea.Select();
    }

    private void OnGotPassword(bool success, string text)
    {
        if (success) 
        { 
            rawPasswordInput = text; 
            PasswordText.text = new string('*', text.Length); 
        }

        LoginButton.Select();
    }

    private async void LoginButtonPressed()
    {
        bool loginSuccess = await ApiUserManager.Login(UsernameText.text, rawPasswordInput);

        if (loginSuccess)
            Close();
        else
            AlertCreator.Instance.CreateNotification("Invalid username and/or password");
    }

    private void Close()
    {
        OnLoginScreenWasExited?.Invoke();
        previousScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);

        if (previousScreen.GetType() == typeof(LevelDetailsScreen))
            ((LevelDetailsScreen)previousScreen).SelectDefaultButton();
    }

    private void CreateNewAccountButtonPressed()
    {

    }
}
