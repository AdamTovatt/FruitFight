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
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

    }

    private void Start()
    {
        DontDestroyOnLoad(this);
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

    [Command(requiresAuthority = false)]
    private void CmdLoadSceneForAll(string sceneName)
    {
        LoadSceneForAll(sceneName);
    }

    [ClientRpc]
    private void LoadSceneOnClient(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneForAll(string sceneName)
    {
        if (!CustomNetworkManager.Instance.IsServer)
        {
            CmdLoadSceneForAll(sceneName);
            return;
        }

        LoadSceneOnClient(sceneName);
    }
}
