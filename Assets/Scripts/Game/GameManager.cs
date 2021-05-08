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

        playerInputManager = gameObject.GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += PlayerJoined;

        Instance = this;
    }

    private void PlayerJoined(PlayerInput playerInput)
    {
        PlayerConfiguration joiningUser = new PlayerConfiguration(playerInput);
        Users.Add(joiningUser);
        PlayerInput[] players = FindObjectsOfType<PlayerInput>();
        PlayerCharacters.Add(joiningUser.Input.transform.GetComponent<PlayerMovement>());
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
