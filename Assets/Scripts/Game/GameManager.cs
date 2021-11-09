using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Linq;
using Mirror;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool ShouldStartLevel;

    public List<PlayerMovement> PlayerCharacters;
    public bool IsDebug = false;
    public GameObject CameraPrefab;
    public GameObject PlayerPrefab;
    public CameraManager CameraManager;
    public int BlockSeeThroughRadius = 2;

    public bool Paused { get; private set; }
    public MultipleTargetCamera MultipleTargetCamera { get; set; }
    public List<PlayerInformation> Players { get; set; }

    public delegate void IsDebugChangedHandler(object sender, bool newState);
    public event IsDebugChangedHandler OnDebugStateChanged;

    private bool oldIsDebug = false;
    private PlayerInputManager playerInputManager;
    private PlayerControls playerControls;
    private NavMeshSurface navMeshSurface;
    private static float currentLevel = 1;

    private bool hasInitializedLevel;

    public void Awake()
    {
        Players = new List<PlayerInformation>();
        PlayerCharacters = new List<PlayerMovement>();
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (Instance == null)
            Instance = this;
    }

    public void Start()
    {
        if (PlayerConfigurationManager.Instance == null)
        {
            SceneManager.LoadScene("PlayerSetup");
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
            Destroy(Instance);

        Instance = this;

        if (ShouldStartLevel)
            StartLevel();

        PlayerControls playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Gameplay.Pause.performed += Pause;
    }

    private void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.Gameplay.Pause.performed -= Pause;
        }
    }

    public void StartLevel()
    {
        WorldBuilder.IsInEditor = false;
        WorldBuilder.Instance.BuildNext();
        navMeshSurface.BuildNavMesh();

        PlayerSpawnpoint playerSpawnpoint = FindObjectOfType<PlayerSpawnpoint>();

        bool addedPlayerCamera = false;

        if (playerSpawnpoint != null)
        {
            if (!CustomNetworkManager.IsOnlineSession) //only if this is an offline session
            {
                PlayerConfiguration[] playerConfigurations = PlayerConfigurationManager.Instance.PlayerConfigurations.ToArray();
                foreach (PlayerConfiguration playerConfiguration in playerConfigurations)
                {
                    playerConfiguration.Input.SwitchCurrentActionMap("Gameplay");

                    GameObject player = Instantiate(PlayerPrefab, playerSpawnpoint.transform.position, playerSpawnpoint.transform.rotation);
                    PlayerMovement playerMovement = player.gameObject.GetComponent<PlayerMovement>();

                    SingleTargetCamera camera = CameraManager.AddCamera(player.transform, playerConfiguration.Input);
                    CameraViewType viewType = playerConfigurations.Length > 1 ? (addedPlayerCamera ? CameraViewType.Right : CameraViewType.Left) : CameraViewType.Full;
                    camera.SetViewType(viewType);
                    addedPlayerCamera = true;
                    WorldBuilder.Instance.AddPreviousWorldObjects(camera.gameObject);

                    GameObject hatPrefab = PrefabKeeper.Instance.GetPrefab(playerConfiguration.GetHatAsPrefabEnum());
                    if (hatPrefab != null)
                    {
                        GameObject playerHat = Instantiate(hatPrefab, playerMovement.GetComponentInChildren<HatPoint>().transform.position, playerMovement.transform.rotation);
                        playerHat.transform.SetParent(playerMovement.transform.GetComponentInChildren<HatPoint>().transform);
                    }

                    playerMovement.InitializePlayerInput(playerConfiguration, camera);
                    PlayerCharacters.Add(playerMovement);

                    Players.Add(new PlayerInformation(playerConfiguration, playerMovement, playerMovement.gameObject.GetComponent<Health>()));
                }

                CreateUiForPlayers();
            }
            else //this is an online session!
            {
                if (CustomNetworkManager.Instance.IsServer)
                {
                    PlayerNetworkIdentity.LocalPlayerInstance.OnReadyStatusUpdated += PlayerReadyStatusWasUpdated;
                    PlayerNetworkIdentity.OtherPlayerInstance.OnReadyStatusUpdated += PlayerReadyStatusWasUpdated;
                }

                PlayerNetworkIdentity.LocalPlayerInstance.SetReady(true);

            }
        }
        else
        {
            CameraManager.AddCamera(transform, null, false);
        }

        GameUi.Instance.HideLoadingScreen();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PlayerReadyStatusWasUpdated(bool localPlayer, bool newStatus)
    {
        if (PlayerNetworkIdentity.OtherPlayerInstance.Ready && PlayerNetworkIdentity.LocalPlayerInstance.Ready)
        {
            InitializeLevelOnline(FindObjectOfType<PlayerSpawnpoint>());
        }
    }

    public void InitializeLevelOnline(PlayerSpawnpoint playerSpawnpoint)
    {
        if (CustomNetworkManager.Instance.IsServer && !hasInitializedLevel)
        {
            //host stuff
            GameObject hostPlayer = Instantiate(PlayerPrefab, playerSpawnpoint.transform.position, playerSpawnpoint.transform.rotation);
            NetworkServer.Spawn(hostPlayer);
            PlayerNetworkCharacter hostNetworkCharacter = hostPlayer.GetComponent<PlayerNetworkCharacter>();
            hostNetworkCharacter.NetId = PlayerNetworkIdentity.LocalPlayerInstance.NetId;

            hostNetworkCharacter.SetupPlayerNetworkCharacter(true);

            //client stuff
            GameObject clientPlayer = Instantiate(PlayerPrefab, playerSpawnpoint.transform.position, playerSpawnpoint.transform.rotation);
            NetworkServer.Spawn(clientPlayer, PlayerNetworkIdentity.OtherPlayerInstance.connectionToClient);
            PlayerNetworkCharacter clientNetworkCharacter = clientPlayer.GetComponent<PlayerNetworkCharacter>();
            clientNetworkCharacter.NetId = PlayerNetworkIdentity.OtherPlayerInstance.NetId;

            clientNetworkCharacter.SetupPlayerNetworkCharacter(false);

            hasInitializedLevel = true;

            PlayerNetworkIdentity.LocalPlayerInstance.OnReadyStatusUpdated -= PlayerReadyStatusWasUpdated;
            PlayerNetworkIdentity.OtherPlayerInstance.OnReadyStatusUpdated -= PlayerReadyStatusWasUpdated;
        }
    }

    public void CreateUiForPlayers()
    {
        foreach (PlayerInformation player in Players)
        {
            GameUi.Instance.CreatePlayerInfoUi(player);
            player.Movement.transform.parent = transform;
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

    public void LevelFinished()
    {
        if (WorldEditor.Instance == null || !WorldEditor.IsTestingLevel)
        {
            currentLevel++;
            WorldBuilder.NextLevel = World.FromWorldName(currentLevel.ToString().PadLeft(2, '0'));
            StartCoroutine(LoadGamePlay());
        }
    }

    private IEnumerator LoadGamePlay()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("GamePlay");
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (!Paused)
        {
            DisablePlayerControls();
            Paused = true;
            GameUi.Instance.ShowPauseMenu();
        }
        else
        {
            GameUi.Instance.PauseMenuWasClosed();
        }
    }

    public void GameWasResumed()
    {
        EnablePlayerControls();
        Paused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetPlayerControls(bool newValue)
    {
        foreach (PlayerMovement player in PlayerCharacters)
        {
            player.ControlsEnabled = newValue;
        }
    }

    public void DisablePlayerControls()
    {
        SetPlayerControls(false);
        CameraManager.DisableCameraInput();
    }

    public void EnablePlayerControls()
    {
        SetPlayerControls(true);
        CameraManager.EnableCameraInput();
    }
}
