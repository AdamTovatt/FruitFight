using UnityEngine;
using UnityEngine.UI;

public class TestLevelPauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button ExitButton;

    private void Start()
    {
        ResumeButton.onClick.AddListener(() => { WorldEditorUi.Instance.CloseLevelTestPauseMenu(); });
        ExitButton.onClick.AddListener(() => { WorldEditorUi.Instance.CloseLevelTestPauseMenu(); WorldEditor.Instance.ExitLevelTest(); });
    }

    public void WasShown()
    {
        foreach (Tween tween in gameObject.GetComponentsInChildren<Tween>())
        {
            tween.WasShown();
        }

        ResumeButton.Select();
    }
}
