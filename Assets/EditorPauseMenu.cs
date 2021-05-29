using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorPauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button TestLevelButton;
    public Button SaveLevelButton;
    public Button LoadLevelButton;
    public Button OptionsButton;
    public Button ExitButton;
    public bool StartActive = false;

    private void Start()
    {
        gameObject.SetActive(StartActive);

        ResumeButton.onClick.AddListener(() => { WorldEditorUi.Instance.ClosePauseMenu(); });
        TestLevelButton.onClick.AddListener(() => { WorldEditor.Instance.TestLevel(); });
        SaveLevelButton.onClick.AddListener(() => { Debug.Log("save level"); });
        LoadLevelButton.onClick.AddListener(() => { Debug.Log("load level"); });
        OptionsButton.onClick.AddListener(() => { Debug.Log("optionsButton"); });
        ExitButton.onClick.AddListener(() => { Debug.Log("exit button"); });
    }

    public void WasShown()
    {
        foreach(Tween tween in gameObject.GetComponentsInChildren<Tween>())
        {
            tween.WasShown();
        }

        ResumeButton.Select();
    }
}
