using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI Title;
    public TextMeshProUGUI DeleteButtonText;

    public MainMenuLocalMenu LocalMenu;
    public CreateProfileScreen CreateProfileScreen;
    public StoryLevelSelectScreen LevelSelectScreen;

    private string originalTitle;
    private string originalDeleteButtonText;

    private void Awake()
    {
        originalDeleteButtonText = DeleteButtonText.text;
        originalTitle = Title.text;
        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        BackButton.onClick.AddListener(() => BackButtonClicked());
        BindProfileButtons();
        DeleteProfileButton.onClick.AddListener(() => DeleteButtonClick());
    }

    private void UnBindEvents()
    {
        BackButton.onClick.RemoveAllListeners();
        UnBindProfileButtons();
        DeleteProfileButton.onClick.RemoveAllListeners();
    }

    private void BindProfileButtons()
    {
        Profile1Button.onClick.AddListener(() => ProfileButtonClick(0));
        Profile2Button.onClick.AddListener(() => ProfileButtonClick(1));
        Profile3Button.onClick.AddListener(() => ProfileButtonClick(2));
    }

    private void UnBindProfileButtons()
    {
        Profile1Button.onClick.RemoveAllListeners();
        Profile2Button.onClick.RemoveAllListeners();
        Profile3Button.onClick.RemoveAllListeners();
    }

    private void DeleteButtonClick()
    {
        Title.text = "Select a profile to delete";
        DeleteButtonText.text = "cancel delete";

        DeleteProfileButton.onClick.RemoveAllListeners();
        DeleteProfileButton.onClick.AddListener(() => CancelDelete());

        UnBindProfileButtons();
        Profile1Button.onClick.AddListener(() => DeleteProfile(0));
        Profile2Button.onClick.AddListener(() => DeleteProfile(1));
        Profile3Button.onClick.AddListener(() => DeleteProfile(2));
    }

    private void CancelDelete()
    {
        DeleteButtonText.text = originalDeleteButtonText;
        UnBindProfileButtons();
        BindProfileButtons();
        DeleteProfileButton.onClick.RemoveAllListeners();
        DeleteProfileButton.onClick.AddListener(() => DeleteButtonClick());
    }

    private void DeleteProfile(int index)
    {
        ProfileSaveState save = SaveProfileHelper.GetSaveState();
        save.RemoveProfile(index);
        SaveProfileHelper.WriteSaveState(save);

        UnBindProfileButtons();
        BindProfileButtons();
        Title.text = originalTitle;
        InitializeButtons();
        DeleteButtonText.text = originalDeleteButtonText;
        DeleteProfileButton.onClick.RemoveAllListeners();
        DeleteProfileButton.onClick.AddListener(() => DeleteButtonClick());
    }

    private void ProfileButtonClick(int index)
    {
        ProfileSave save = SaveProfileHelper.GetSaveState().GetProfile(index);

        if (save.EmptyProfile)
        {
            CreateProfileScreen.gameObject.SetActive(true);
            CreateProfileScreen.Show(index);
            gameObject.SetActive(false);
        }
        else
        {
            ShowLevelSelectScreen(index);
        }
    }

    private void ShowLevelSelectScreen(int profileIndex)
    {
        Debug.Log("Set current save profile");
        SaveProfileHelper.CurrentProfileIndex = profileIndex;
        LevelSelectScreen.gameObject.SetActive(true);
        LevelSelectScreen.Show();
        gameObject.SetActive(false);
    }

    private void InitializeButtons()
    {
        ProfileSaveState saveState = SaveProfileHelper.GetSaveState();

        Profile1.Initialize(saveState.GetProfile(0));
        Profile2.Initialize(saveState.GetProfile(1));
        Profile3.Initialize(saveState.GetProfile(2));
    }

    public void Show()
    {
        InitializeButtons();

        Profile1Button.Select();
    }

    public void BackButtonClicked()
    {
        LocalMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
        LocalMenu.Show();
    }
}
