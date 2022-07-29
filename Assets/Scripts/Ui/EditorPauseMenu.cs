using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorPauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button TestLevelButton;
    public Button SaveLevelButton;
    public Button LoadLevelButton;
    public Button LevelPropertiesButton;
    public Button ExitButton;

    private Tween[] tweens;

    private void Awake()
    {
        tweens = gameObject.GetComponentsInChildren<Tween>();
    }

    private void Start()
    {
        ResumeButton.onClick.AddListener(() => { WorldEditorUi.Instance.ClosePauseMenu(); });
        TestLevelButton.onClick.AddListener(() => { WorldEditor.Instance.TestLevelButton(); });
        SaveLevelButton.onClick.AddListener(() => { WorldEditor.Instance.SaveLevel(); });
        LoadLevelButton.onClick.AddListener(() => { WorldEditor.Instance.LoadLevel(); });
        LevelPropertiesButton.onClick.AddListener(WorldEditorUi.Instance.ShowLevelProperties);
        ExitButton.onClick.AddListener(() => { WorldEditor.Instance.ExitButtonWasPressed(); });
    }

    public void WasShown()
    {
        foreach (Tween tween in tweens)
        {
            tween.WasShown();
        }

        ResumeButton.Select();
    }
}
