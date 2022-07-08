using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryLevelContainer : MonoBehaviour
{
    public Button NextButton;
    public Button PreviousButton;
    public GameObject LevelButtonPrefab;

    public int Rows;
    public int Columns;

    private List<World> levels;
    private ProfileSave save;

    private int currentPage = 1;

    private List<StoryLevelButton> currentButtons = new List<StoryLevelButton>();

    public void Initialize(List<World> levels, ProfileSave save)
    {
        this.levels = levels;
        this.save = save;
    }

    public List<StoryLevelButton> DisplayPage()
    {
        return DisplayPage(currentPage);
    }

    public List<StoryLevelButton> DisplayPage(int page)
    {
        foreach (StoryLevelButton button in currentButtons)
        {
            Destroy(button.gameObject);
        }
        currentButtons.Clear();

        foreach (World level in levels)
        {
            StoryLevelButton button = Instantiate(LevelButtonPrefab, new Vector3(0, 0, 0), gameObject.transform.rotation, gameObject.transform).GetComponent<StoryLevelButton>();

            Sprite buttonSprite = button.CreateScaledSpriteFromTexture(level.Metadata.GetImageDataAsTexture2d());

            button.Initialize(level.Metadata.Name, buttonSprite, level.StoryModeLevelEntry.Id == 1 || save.HasCompletedLevel(level.StoryModeLevelEntry.Id - 1)); //determine if the button is enabled
            button.Button.onClick.AddListener(() => { LevelWasClicked(level); });
            currentButtons.Add(button);
        }

        currentPage = page;

        return currentButtons;
    }

    private void LevelWasClicked(World level)
    {
        WorldBuilder.NextLevel = level;
        Debug.Log("Level clicked: " + level.Metadata.Name);

        GameStateManager.SetGameState(GameState.Story);

        if (!CustomNetworkManager.IsOnlineSession)
        {
            DontDestroyOnLoad(MainMenuUi.Instance.gameObject);
            MainMenuUi.Instance.MouseOverSelectableChecker.Disable();
            MainMenuUi.Instance.LoadingScreen.gameObject.SetActive(true);
            WorldBuilder.NextLevel = level;
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadScene("GamePlay");
        }
        else
        {
            if (level.Metadata.Id == 0)
            {
                AlertCreator.Instance.CreateNotification("Only levels from the online library can be played in online multiplayer");
            }
            else
            {
                NetworkMethodCaller.Instance.RpcClientShouldStartLevel(level.Metadata.Id);
            }
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MainMenuUi.Instance.LoadingScreen.gameObject.SetActive(false);
        MainMenuUi.Instance.gameObject.SetActive(false);
        SceneManager.sceneLoaded -= SceneLoaded;
    }
}
