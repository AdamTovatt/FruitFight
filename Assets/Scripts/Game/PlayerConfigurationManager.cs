using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    public int MaxPlayers = 2;
    public GameObject GameManagerPrefab;
    public GameObject JoinInstructionsText;
    public GameObject PlayerSetupPanelPrefab;
    public PlayerConfigurationOnlineManager OnlineManager;
    public LoadingScreen LoadingScreen;

    public InputMode CurrentInputMode { get; private set; }
    public List<PlayerConfiguration> PlayerConfigurations { get; private set; }
    private PlayerInputManager playerInputManager;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            throw new Exception("Instance of PlayerConfigurationManager already exists");
        }
        else
        {
            playerInputManager = gameObject.GetComponent<PlayerInputManager>();
            playerInputManager.onPlayerJoined += HandlePlayerJoin;
            playerInputManager.onPlayerLeft += HandlePlayerLeft;

            Instance = this;
            DontDestroyOnLoad(Instance);
            PlayerConfigurations = new List<PlayerConfiguration>();

            LoadingScreen.Hide();

            if (CustomNetworkManager.IsOnlineSession)
            {
                if (CustomNetworkManager.Instance.IsServer)
                {
                    playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
                    LoadingScreen.Show();
                }
                else
                {
                    NetworkMethodCaller.Instance.CmdEnablePlayerConfigurationManagerForHost();
                }
            }
        }
    }

    public void EnableForHost()
    {
        LoadingScreen.Hide();
        playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
    }

    public void SetPlayerHat(int index, int hat)
    {
        PlayerConfigurations[index].Hat = hat;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == "GamePlay")
        {
            Instantiate(GameManagerPrefab, transform.position, transform.rotation);
            GameManager.ShouldStartLevel = true;
        }
    }

    public void ReadyPlayer(int index, Texture2D playerPortrait, bool isLocalPlayer)
    {
        if (!CustomNetworkManager.IsOnlineSession)
        {
            PlayerConfigurations[index].IsReady = true;
            PlayerConfigurations[index].Portrait = playerPortrait;

            if (PlayerConfigurations.All(p => p.IsReady == true))
            {
                SceneManager.LoadScene("GamePlay");
            }
        }
        else
        {
            if (isLocalPlayer)
                PlayerNetworkIdentity.LocalPlayerInstance.Portrait = playerPortrait;
            else
                PlayerNetworkIdentity.OtherPlayerInstance.Portrait = playerPortrait;

            if(CustomNetworkManager.Instance.IsServer && PlayerNetworkIdentity.LocalPlayerInstance.Ready && PlayerNetworkIdentity.OtherPlayerInstance.Ready)
            {
                for (int i = 0; i < PlayerConfigurations.Count; i++)
                {
                    UnReadyPlayer(i);
                }

                PlayerNetworkIdentity.LocalPlayerInstance.SetReady(false);
                PlayerNetworkIdentity.OtherPlayerInstance.SetReady(false);

                NetworkMethodCaller.Instance.LoadSceneForAll("GamePlay");
            }
        }
    }

    public void UnReadyPlayer(int index)
    {
        PlayerConfigurations[index].IsReady = false;
    }

    public void PlayerSetupMenuWasCreatedLocally(GameObject menu)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                NetworkServer.Spawn(menu);
            }
            else
            {
                PlayerNetworkIdentity.LocalPlayerInstance.JoinPlayerOnServer();
            }
        }
    }

    public void ClientJoinSetupScreen()
    {
        GameObject rootMenu = GameObject.Find("MainLayout");
        GameObject menu = Instantiate(PlayerSetupPanelPrefab, rootMenu.transform);
        NetworkServer.Spawn(menu);
        PlayerSetupMenuController menuController = menu.GetComponent<PlayerSetupMenuController>();
        menuController.AquireControl();
    }

    private void HandlePlayerJoin(PlayerInput playerInput)
    {
        if (JoinInstructionsText != null)
            Destroy(JoinInstructionsText);

        if(!PlayerConfigurations.Any(p => p.PlayerIndex == playerInput.playerIndex))
        {
            playerInput.transform.SetParent(transform);
            playerInput.SwitchCurrentActionMap(InputModeEnumToString(CurrentInputMode));
            PlayerConfigurations.Add(new PlayerConfiguration(playerInput));
        }

        this.CallWithDelay(RefreshUiModule, 0.5f); //a bug in unity requires us to disable and then enable the ui input module
    }

    private void RefreshUiModule()
    {
        foreach (PlayerConfiguration config in PlayerConfigurations)
        {
            if (config != null && config.Input != null && config.Input.uiInputModule != null)
            {
                config.Input.uiInputModule.enabled = false;
                config.Input.uiInputModule.enabled = true;
            }
        }
    }

    public void SetInputMode(InputMode mode)
    {
        string inputModeName = InputModeEnumToString(mode);

        foreach(PlayerConfiguration playerConfiguration in PlayerConfigurations.ToArray())
        {
            playerConfiguration.Input.SwitchCurrentActionMap(inputModeName);
        }

        CurrentInputMode = mode;
    }

    private string InputModeEnumToString(InputMode inputModeEnum)
    {
        switch (inputModeEnum)
        {
            case InputMode.Gameplay:
                return "Gameplay";
            case InputMode.Ui:
                return "Ui";
            default:
                throw new Exception("No input mode with name: " + inputModeEnum.ToString());
        }
    }

    private void HandlePlayerLeft(PlayerInput playerInput)
    {
        PlayerConfigurations = PlayerConfigurations.Where(x => x.PlayerIndex != playerInput.playerIndex).ToList();
        RefreshUiModule(); //just in case unity pranks us
    }
}

public enum InputMode
{
    Gameplay, Ui
}