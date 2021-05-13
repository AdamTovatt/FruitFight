using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        if (PlayerConfigurationManager.Instance == null)
        {
            SceneManager.LoadScene("PlayerSetup");
            return;
        }
    }

    public void StartLevel()
    {
        WorldBuilder.Instance.Build("01");

        PlayerSpawnpoint playerSpawnpoint = GameObject.FindObjectOfType<PlayerSpawnpoint>();

        foreach (PlayerConfiguration playerConfiguration in PlayerConfigurationManager.Instance.PlayerConfigurations.ToArray())
        {
            playerConfiguration.Input.SwitchCurrentActionMap("Gameplay");
            GameObject player = Instantiate(PlayerPrefab, playerSpawnpoint.transform.position, playerSpawnpoint.transform.rotation);
            PlayerMovement playerMovement = player.gameObject.GetComponent<PlayerMovement>();

            GameObject hatPrefab = PrefabKeeper.Instance.GetPrefab(playerConfiguration.GetHatAsPrefabEnum());
            if (hatPrefab != null)
            {
                GameObject playerHat = Instantiate(hatPrefab, playerMovement.GetComponentInChildren<HatPoint>().transform.position, playerMovement.transform.rotation);
                playerHat.transform.SetParent(playerMovement.transform.GetComponentInChildren<HatPoint>().transform);
            }

            playerMovement.InitializePlayerInput(playerConfiguration);
            PlayerCharacters.Add(playerMovement);
        }
    }

    public void Update()
    {
        if (oldIsDebug != IsDebug)
        {
            OnDebugStateChanged?.Invoke(this, IsDebug);
        }

        oldIsDebug = IsDebug;
    }
}
