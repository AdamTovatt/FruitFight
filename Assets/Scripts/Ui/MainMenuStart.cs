using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuStart : MonoBehaviour
{
    public Button PlayButton;
    public Button LevelEditorButton;
    public Button BrowseLevelsButton;
    public Button OptionsButton;
    public Button ExitButton;

    public MainMenuPlayMenu PlayMenu;

    public MainMenuUi Ui;

    private void Start()
    {
        PlayButton.onClick.AddListener(Play);
        LevelEditorButton.onClick.AddListener(Ui.LevelEditorButtonWasPressed);
        BrowseLevelsButton.onClick.AddListener(Ui.BrowseLevelsButtonWasPressed);
        ExitButton.onClick.AddListener(Application.Quit);
    }

    private void Play()
    {
        transform.SetParent(null);
        PlayMenu.gameObject.SetActive(true);
        PlayMenu.Show(this);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        PlayButton.Select();
    }
}
