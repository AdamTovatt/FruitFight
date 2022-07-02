using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLocalMenu : MonoBehaviour
{
    public Button StoryModeButton;
    public Button BrowseLevelsButton;
    public Button BackButton;

    public StoryModeScreen StoryModeScreen;
    private MainMenuPlayMenu previousMenu;

    void Start()
    {
        StoryModeButton.onClick.AddListener(Story);
        BrowseLevelsButton.onClick.AddListener(BrowseLevels);
        BackButton.onClick.AddListener(Back);
    }

    private void BrowseLevels()
    {
        MainMenuUi.Instance.BrowseLevelsButtonWasPressed();
        gameObject.SetActive(false);
    }

    private void Story()
    {
        GameStateManager.SetGameState(GameState.Story);
        StoryModeScreen.gameObject.SetActive(true);
        StoryModeScreen.Show();
        //MainMenuUi.Instance.PlayButtonWasPressed();
        gameObject.SetActive(false);
    }

    private void Back()
    {
        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);
        gameObject.SetActive(false);
    }

    public void Show(MainMenuPlayMenu previousMenu)
    {
        this.previousMenu = previousMenu;
        previousMenu.gameObject.SetActive(false);
        StoryModeButton.Select();
    }

    public void Show()
    {
        StoryModeButton.Select();
    }
}
