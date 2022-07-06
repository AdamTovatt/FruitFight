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
        List<StoryLevelButton> buttons = LevelContainer.DisplayPage(1);

        if (buttons.Count > 0)
            this.CallWithDelay(() => { SelectButton(buttons[0].Button); }, 0.1f);
    }

    private void SelectButton(Button button)
    {
        button.Select();
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
