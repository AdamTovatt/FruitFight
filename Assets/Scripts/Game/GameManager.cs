using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<User> Users;
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
        Users = new List<User>();
        PlayerCharacters = new List<PlayerMovement>();

        playerInputManager = gameObject.GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += PlayerJoined;

        Instance = this;
    }

    private void PlayerJoined(PlayerInput playerInput)
    {
        User joiningUser = new User(true) { ConnectedInput = playerInput };
        Users.Add(joiningUser);
        PlayerInput[] players = FindObjectsOfType<PlayerInput>();
        PlayerCharacters.Add(joiningUser.ConnectedInput.transform.GetComponent<PlayerMovement>());
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
