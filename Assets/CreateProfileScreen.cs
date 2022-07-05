using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateProfileScreen : MonoBehaviour
{
    public TMP_InputField NameInput;
    public Button CreateButton;
    public Button CancelButton;
    public StoryModeScreen StoryModeScreen;

    private ProfileSave currentProfile;

    private void Awake()
    {
        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    public void Show(int stateIndex)
    {
        currentProfile = SaveProfileHelper.GetSaveState().GetProfile(stateIndex);
        NameInput.text = "";
        NameInput.Select();
    }

    private void BindEvents()
    {
        CreateButton.onClick.AddListener(() => Create());
        CancelButton.onClick.AddListener(() => Cancel());
        NameInput.onEndEdit.AddListener((text) => { if (text == "") CancelButton.Select(); else CreateButton.Select(); });
    }

    private void UnBindEvents()
    {
        CreateButton.onClick.RemoveAllListeners();
        CancelButton.onClick.RemoveAllListeners();
        NameInput.onEndEdit.RemoveAllListeners();
    }

    private void Cancel()
    {
        StoryModeScreen.gameObject.SetActive(true);
        StoryModeScreen.Show();
        gameObject.SetActive(false);
        currentProfile = null;
    }

    private void Create()
    {
        currentProfile.Name = NameInput.text;
        currentProfile.EmptyProfile = false;
        SaveProfileHelper.WriteSaveState(SaveProfileHelper.GetSaveState());

        Cancel();
    }
}
