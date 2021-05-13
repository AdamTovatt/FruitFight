using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    public int MaxPlayers = 2;
    public GameObject GameManagerPrefab;

    public InputMode CurrentInputMode { get; private set; }
    public List<PlayerConfiguration> PlayerConfigurations { get; private set; }
    private PlayerInputManager playerInputManager;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            throw new System.Exception("Instance of PlayerConfigurationManager already exists");
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
        Debug.Log("hat: " + hat);
        PlayerConfigurations[index].Hat = hat;
    }

    public void ReadyPlayer(int index)
    {
        PlayerConfigurations[index].IsReady = true;
        if(PlayerConfigurations.All(p => p.IsReady == true))
        {
            SceneManager.LoadScene("SampleScene");
        }

        SceneManager.sceneLoaded += (scene, loadMode) => 
        {
            Instantiate(GameManagerPrefab, transform.position, transform.rotation);
            GameManager.ShouldStartLevel = true; 
        };
    }

    public void UnReadyPlayer(int index)
    {
        PlayerConfigurations[index].IsReady = false;
    }

    private void HandlePlayerJoin(PlayerInput playerInput)
    {
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
                throw new System.Exception("No input mode with name: " + inputModeEnum.ToString());
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