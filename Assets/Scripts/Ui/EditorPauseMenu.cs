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

    private void Start()
    {
        ResumeButton.onClick.AddListener(() => { WorldEditorUi.Instance.ClosePauseMenu(); });
        TestLevelButton.onClick.AddListener(() => { WorldEditor.Instance.TestLevelButton(); });
        SaveLevelButton.onClick.AddListener(() => { WorldEditor.Instance.SaveLevel(); });
        LoadLevelButton.onClick.AddListener(() => { WorldEditor.Instance.LoadLevel(); });
        OptionsButton.onClick.AddListener(() => { Debug.Log("optionsButton"); });
        ExitButton.onClick.AddListener(() => { WorldEditor.Instance.ExitButtonWasPressed(); });
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
