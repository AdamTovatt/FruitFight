using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button RestartLevelButton;
    public Button ViewControlsButton;
    public Button OptionsButton;
    public Button ExitLevelButton;

    public delegate void OnClosedHandler();
    public event OnClosedHandler OnClosed;

    public delegate void OnExitLevelHandler();
    public event OnExitLevelHandler OnExitLevel;

    public delegate void OnRestartLevelHandler();
    public event OnRestartLevelHandler OnRestartLevel;

    private void Start()
    {
        ResumeButton.onClick.AddListener(Closed);
        ExitLevelButton.onClick.AddListener(ExitLevel);
    }

    public void Show()
    {
        ResumeButton.Select();
    }

    private void ExitLevel()
    {
        OnExitLevel?.Invoke();
        OnExitLevel = null;
    }

    public void Close()
    {
        Closed();
    }

    private void Closed()
    {
        OnClosed?.Invoke();
        OnClosed = null;
    }

    private void RestartLevel()
    {
        OnRestartLevel?.Invoke();
        OnRestartLevel = null;
    }
}
