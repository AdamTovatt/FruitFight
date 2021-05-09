using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    public int MaxPlayers = 2;

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
    }

    private void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("Player joined: " + playerInput.playerIndex);

        if(!PlayerConfigurations.Any(p => p.PlayerIndex == playerInput.playerIndex))
        {
            playerInput.transform.SetParent(transform);
            PlayerConfigurations.Add(new PlayerConfiguration(playerInput));
        }
    }

    private void HandlePlayerLeft(PlayerInput obj)
    {
        throw new System.NotImplementedException();
    }
}