using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryLevelSelectScreen : MonoBehaviour
{
    public Button BackButton;
    public StoryLevelContainer LevelContainer;

    public StoryModeScreen StoryModeScreen;

    public void Show()
    {
        BackButton.Select();

        ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();

        List<World> levels = WorldUtilities.GetStoryModeLevels();

        LevelContainer.Initialize(levels, save);
        LevelContainer.DisplayPage(1);
    }

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
        BackButton.onClick.AddListener(() => Back());
    }

    private void UnBindEvents()
    {
        BackButton.onClick.RemoveAllListeners();
    }

    private void Back()
    {
        StoryModeScreen.gameObject.SetActive(true);
        StoryModeScreen.Show();
        gameObject.SetActive(false);
    }
}
