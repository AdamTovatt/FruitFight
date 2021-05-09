using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerConfiguration> Users;
    public List<PlayerMovement> PlayerCharacters;
    public Camera Camera;
    public bool IsDebug = false;
    public GameObject PlayerPrefab;

    public delegate void IsDebugChangedHandler(object sender, bool newState);
    public event IsDebugChangedHandler OnDebugStateChanged;

    private bool oldIsDebug = false;
    private PlayerInputManager playerInputManager;

    public void Awake()
    {
        Users = new List<PlayerConfiguration>();
        PlayerCharacters = new List<PlayerMovement>();

        Instance = this;
    }

    public void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        PlayerSpawnpoint playerSpawnpoint = GameObject.FindObjectOfType<PlayerSpawnpoint>();

        foreach(PlayerConfiguration playerConfiguration in PlayerConfigurationManager.Instance.PlayerConfigurations.ToArray())
        {
            playerConfiguration.Input.SwitchCurrentActionMap("Gameplay");
            GameObject player = Instantiate(PlayerPrefab, playerSpawnpoint.transform.position, playerSpawnpoint.transform.rotation);
            PlayerMovement playerMovement = player.gameObject.GetComponent<PlayerMovement>();
            playerMovement.InitializePlayerInput(playerConfiguration);
            PlayerCharacters.Add(playerMovement);
        }
    }

    public void Update()
    {
        if(oldIsDebug != IsDebug)
        {
            OnDebugStateChanged?.Invoke(this, IsDebug);
        }

        oldIsDebug = IsDebug;
    }
}
