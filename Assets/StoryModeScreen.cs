using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryModeScreen : MonoBehaviour
{
    public Button BackButton;
    public Button DeleteProfileButton;
    public Button Profile1Button;
    public Button Profile2Button;
    public Button Profile3Button;
    public ProfileButton Profile1;
    public ProfileButton Profile2;
    public ProfileButton Profile3;

    public MainMenuLocalMenu LocalMenu;

    private void Awake()
    {
        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        BackButton.onClick.AddListener(() => BackButtonClicked());
    }

    private void UnBindEvents()
    {
        BackButton.onClick.RemoveAllListeners();
    }

    public void Show()
    {
        ProfileSaveState saveState = SaveProfileHelper.GetSaveState();

        Profile1.Initialize(saveState.GetProfile(0));
        Profile2.Initialize(saveState.GetProfile(1));
        Profile3.Initialize(saveState.GetProfile(2));

        Profile1Button.Select();
    }

    public void BackButtonClicked()
    {
        LocalMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
        LocalMenu.Show();
    }
}
