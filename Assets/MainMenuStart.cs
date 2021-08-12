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

    public MainMenuUi Ui;

    private void Start()
    {
        PlayButton.onClick.AddListener(Ui.PlayButtonWasPressed);
        LevelEditorButton.onClick.AddListener(Ui.LevelEditorButtonWasPressed);
    }
}
