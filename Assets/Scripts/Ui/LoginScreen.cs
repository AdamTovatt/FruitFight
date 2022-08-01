using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public TMP_InputField UsernameInputArea;
    public TMP_InputField PasswordInputArea;

    public LoadingSpinnerButton LoginButton;
    public Button CreateNewAccountButton;
    public Button CloseButton;

    public UiKeyboardInput Keyboard;

    public CreateAccountScreen CreateAccountScreen;

    public delegate void LoginScreenWasExited();
    public event LoginScreenWasExited OnLoginScreenWasExited;

    private MonoBehaviour previousScreen;
    private string rawPasswordInput;

    private void Start()
    {
        UsernameInputArea.onEndEdit.AddListener(GotUsername);
        PasswordInputArea.onEndEdit.AddListener(GotPassword);
        LoginButton.Button.onClick.AddListener(LoginButtonPressed);
        CreateNewAccountButton.onClick.AddListener(CreateNewAccountButtonPressed);
        CloseButton.onClick.AddListener(Close);
    }

    public void SelectDefaultButton()
    {
        UsernameInputArea.Select();
    }

    public void Show(MonoBehaviour previousScreen)
    {
        this.previousScreen = previousScreen;
        previousScreen.gameObject.SetActive(false);
        SelectDefaultButton();
    }

    private void GotUsername(string text)
    {
        if (!string.IsNullOrEmpty(text))
            PasswordInputArea.Select();
    }

    private void GotPassword(string text)
    {
        rawPasswordInput = text;

        if (!string.IsNullOrEmpty(text))
            LoginButton.Button.Select();
    }

    private async void LoginButtonPressed()
    {
        if(string.IsNullOrEmpty(UsernameInputArea.text))
        {
            AlertCreator.Instance.CreateNotification("Username can not be empty");
            return;
        }

        if(string.IsNullOrEmpty(rawPasswordInput))
        {
            AlertCreator.Instance.CreateNotification("Password can not be empty");
            return;
        }

        LoginButton.ShowSpinner();
        bool loginSuccess = await ApiUserManager.Login(UsernameInputArea.text, rawPasswordInput);
        LoginButton.ReturnToNormal();

        if (loginSuccess)
            Close();
        else
            AlertCreator.Instance.CreateNotification("Invalid username and/or password");
    }

    public void CloseFromCreateAccountScreen()
    {
        Close();
    }

    private void Close()
    {
        OnLoginScreenWasExited?.Invoke();
        OnLoginScreenWasExited = null;
        previousScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);

        if (previousScreen.GetType() == typeof(LevelDetailsScreen))
            ((LevelDetailsScreen)previousScreen).SelectDefaultButton();
    }

    private void CreateNewAccountButtonPressed()
    {
        CreateAccountScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);

        CreateAccountScreen.Show(this);
    }
}
