using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkMethodCaller : NetworkBehaviour
{
    public static NetworkMethodCaller Instance;
    private Dictionary<Vector3, BouncyObject> bouncyObjects = new Dictionary<Vector3, BouncyObject>();

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

    public GameObject Instantiate(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        if (CustomNetworkManager.Instance.IsServer)
        {
            GameObject instantiatedObject = PerformInstantiate(prefabIndex, position, rotation);
            NetworkServer.Spawn(instantiatedObject);
            return instantiatedObject;
        }
        else
        {
            CmdInstaniate(prefabIndex, position, rotation);
            return PerformInstantiate(prefabIndex, position, rotation);
        }
    }

    private GameObject PerformInstantiate(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        return Instantiate(CustomNetworkManager.Instance.spawnPrefabs[prefabIndex], position, rotation);
    }

    [Command(requiresAuthority = false)]
    private void CmdInstaniate(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        PerformInstantiate(prefabIndex, position, rotation);
    }

    [ClientRpc]
    public async void RpcClientShouldStartLevel(long levelId)
    {
        GetLevelResponse response = await ApiLevelManager.GetLevel(levelId);
        World world = World.FromJson(response.WorldData);
        world.Metadata = response.Metadata;

        StartLevel(world);
    }

    [ClientRpc]
    public void RpcClientShouldStartStoryLevel(string levelName)
    { 
        Debug.Log("Start rpc");
        StartLevel(WorldUtilities.GetStoryModeLevel(levelName));
    }

    [ClientRpc]
    public void RpcLoadGameplay(string levelName)
    {
        WorldBuilder.NextLevel = WorldUtilities.GetStoryModeLevel(levelName);
        Debug.Log("Next level: " + WorldBuilder.NextLevel.Metadata.Name);
        GameManager.Instance.LoadGamePlay();
    }

    private void StartLevel(World world)
    {
        WorldBuilder.NextLevel = world;
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

    public void ClearBouncyObjects()
    {
        bouncyObjects.Clear();
    }

    public void RegisterBouncyObject(BouncyObject bouncyObject)
    {
        bouncyObjects.Add(bouncyObject.transform.position, bouncyObject);
    }

    [Command(requiresAuthority = false)]
    private void CmdBouncyObjectBounced(Vector3 position, float bounceAmplitudeModifier)
    {
        PerformBouncyObjectBounced(position, bounceAmplitudeModifier);
    }

    [ClientRpc]
    private void RpcBouncyObjectBounced(Vector3 position, float bounceAmplitudeModifier)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformBouncyObjectBounced(position, bounceAmplitudeModifier);
    }

    private void PerformBouncyObjectBounced(Vector3 position, float bounceAmplitudeModifier)
    {
        if (bouncyObjects.ContainsKey(position))
            bouncyObjects[position].Bounce(bounceAmplitudeModifier);
    }

    public void BouncyObjectBounced(Vector3 position, float bounceAmplitudeModifier)
    {
        if (CustomNetworkManager.Instance.IsServer)
        {
            RpcBouncyObjectBounced(position, bounceAmplitudeModifier);
        }
        else
        {
            CmdBouncyObjectBounced(position, bounceAmplitudeModifier);
        }
    }

    public void ShowWinScreen(int earnedCoins, int earnedJellyBeans, int earnedXp)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcShowWinScreen(earnedCoins, earnedJellyBeans, earnedXp);
                PerformShowWinScreen(earnedCoins, earnedJellyBeans, earnedXp);
            }
            else
            {
                throw new System.Exception("ShowWinScreen should only be called from the server");
            }
        }
        else
        {
            PerformShowWinScreen(earnedCoins, earnedJellyBeans, earnedXp);
        }
    }

    [ClientRpc]
    private void RpcShowWinScreen(int earnedCoins, int earnedJellyBeans, int earnedXp)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformShowWinScreen(earnedCoins, earnedJellyBeans, earnedXp);
    }

    private void PerformShowWinScreen(int earnedCoins, int earnedJellyBeans, int earnedXp)
    {
        GameUi.Instance.ShowWinScreen(earnedCoins, earnedJellyBeans, earnedXp);
    }

    [Command(requiresAuthority = false)]
    public void CmdContinueFromLevel()
    {
        GameManager.Instance.ContinueFromLevel();
    }

    public void GoToNextStoryLevel(string name)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcGoToNextStoryLevel(name);
                PerformGoToNextStoryLevel(name);
            }
            else
            {
                CmdGoToNextStoryLevel(name);
                PerformGoToNextStoryLevel(name);
            }
        }
        else
        {
            PerformGoToNextStoryLevel(name);
        }
    }

    [ClientRpc]
    private void RpcGoToNextStoryLevel(string name)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformGoToNextStoryLevel(name);
    }

    [Command(requiresAuthority = false)]
    private void CmdGoToNextStoryLevel(string name)
    {
        PerformGoToNextStoryLevel(name);
    }

    private void PerformGoToNextStoryLevel(string name)
    {
        GameManager.Instance.LoadNextStoryLevel(name);
    }

    public void ExitLevel()
    {
        if (CustomNetworkManager.Instance.IsServer)
        {
            RpcExitLevel();
            PerformExitLevel();
        }
        else
        {
            CmdExitLevel();
            PerformExitLevel();
        }
    }

    [ClientRpc]
    private void RpcExitLevel()
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;
        PerformExitLevel();
    }

    [Command(requiresAuthority = false)]
    private void CmdExitLevel()
    {
        PerformExitLevel();
    }

    private void PerformExitLevel()
    {
        GameUi.Instance.ExitLevel();
    }
}
