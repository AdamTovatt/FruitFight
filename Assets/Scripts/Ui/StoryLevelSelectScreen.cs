using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryLevelSelectScreen : MonoBehaviour
{
    public Button BackButton;
    public TextMeshProUGUI InformationText;
    public StoryLevelContainer LevelContainer;

    public StoryModeScreen StoryModeScreen;

    private string menuScene;

    private bool isShown;

    public void Show()
    {
        isShown = true;

        BackButton.Select();

        ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();

        List<World> levels = WorldUtilities.GetStoryModeLevels();

        InformationText.text = string.Format("{0}\n{1}", save.Name, save.ToString());

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
        menuScene = SceneManager.GetActiveScene().name;
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        BackButton.onClick.AddListener(() => Back());
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == menuScene)
        {
            if (isShown)
            {
                gameObject.SetActive(true);

                List<StoryLevelButton> buttons = LevelContainer.DisplayPage();

                BackButton.Select();
            }
        }
    }

    private void UnBindEvents()
    {
        BackButton.onClick.RemoveAllListeners();
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void Back()
    {
        isShown = false;
        StoryModeScreen.gameObject.SetActive(true);
        StoryModeScreen.Show();
        gameObject.SetActive(false);
    }
}
