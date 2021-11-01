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
    public PlayerConfigurationOnlineManager OnlineManager;

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
        }
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

    public void ReadyPlayer(int index, Texture2D playerPortrait)
    {
        PlayerConfigurations[index].IsReady = true;
        PlayerConfigurations[index].Portrait = playerPortrait;
        if(PlayerConfigurations.All(p => p.IsReady == true))
        {
            SceneManager.LoadScene("GamePlay");
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
            Debug.Log(menu);

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
    }
}

public enum InputMode
{
    Gameplay, Ui
}