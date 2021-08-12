using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : UiManager
{
    public static MainMenuUi Instance;

    public LoadingScreen LoadingScreen;
    public MainMenuStart StartMenu;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(ShowMenuWithDelay());
    }

    private IEnumerator ShowMenuWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        StartMenu.gameObject.SetActive(true);
        LoadingScreen.Hide();
        StartMenu.PlayButton.Select();
    }

    public void LevelEditorButtonWasPressed()
    {
        LoadingScreen.Show();
        SceneManager.LoadScene("LevelEditor");
        Instance = null;
    }

    public void PlayButtonWasPressed()
    {
        LoadingScreen.Show();
        SceneManager.LoadScene("GamePlay");
        Instance = null;
    }
}
