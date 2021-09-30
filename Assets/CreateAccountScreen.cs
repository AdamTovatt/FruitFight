using System.Collections;
using System.Collections.Generic;
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

    private LoginScreen parentScreen;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
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

    private void Close()
    {
        this.gameObject.SetActive(false);
        parentScreen.gameObject.SetActive(true);
        parentScreen.SelectDefaultButton();
    }
}
