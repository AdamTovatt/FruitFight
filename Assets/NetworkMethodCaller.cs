using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkMethodCaller : NetworkBehaviour
{
    public static NetworkMethodCaller Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void RpcClientShouldStartStoryLevel(string levelName)
    {
        WorldBuilder.NextLevel = World.FromWorldName(levelName);
        MainMenuUi.Instance.MouseOverSelectableChecker.Disable();
        MainMenuUi.Instance.LoadingScreen.Show();
        SceneManager.LoadScene("GamePlay");
        SceneManager.sceneLoaded += LoadSceneWasCompleted;
    }

    private void LoadSceneWasCompleted(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadSceneWasCompleted;
    }
}
