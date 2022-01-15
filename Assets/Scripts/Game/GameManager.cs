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
    public Dictionary<Transform, Health> TransformsWithHealth { get; set; } = new Dictionary<Transform, Health>();

    public delegate void IsDebugChangedHandler(object sender, bool newState);
    public event IsDebugChangedHandler OnDebugStateChanged;

    private bool oldIsDebug = false;
    private PlayerInputManager playerInputManager;
    private PlayerControls playerControls;
    private NavMeshSurface navMeshSurface;
    private static float currentLevel = 1;

    private bool hasInitializedLevel;

    private List<Spawner> spawners = new List<Spawner>();

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
        Debug.Log("player characters count: " + PlayerCharacters.Count);

        if (CustomNetworkManager.IsOnlineSession && NetworkMethodCaller.Instance != null)
            NetworkMethodCaller.Instance.ClearBouncyObjects(); //clear the dictionary with bouncy objects so it can be filled again with the ones from this level

        TransformsWithHealth.Clear();

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

                    GameObject hatPrefab = HatConfiguration.GetHatPrefab(playerConfiguration.Hat);
                    if (hatPrefab != null)
                    {
                        GameObject playerHat = Instantiate(hatPrefab, playerMovement.GetComponentInChildren<HatPoint>().transform.position, playerMovement.transform.rotation);
                        playerHat.transform.SetParent(playerMovement.transform.GetComponentInChildren<HatPoint>().transform);
                    }

                    playerMovement.InitializePlayerInput(playerConfiguration, camera);
                    playerMovement.MagicProjectileId = HatConfiguration.Hats[playerConfiguration.Hat].MagicProjectileId;
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

    public void CleanLevel()
    {
        foreach (PlayerMovement player in PlayerCharacters)
        {
            Destroy(player.gameObject);
        }
    }

    private void PlayerReadyStatusWasUpdated(bool localPlayer, bool newStatus)
    {
        if (PlayerNetworkIdentity.OtherPlayerInstance.Ready && PlayerNetworkIdentity.LocalPlayerInstance.Ready)
        {
            InitializeLevelOnline(FindObjectOfType<PlayerSpawnpoint>());
        }
    }

    public void AddSpawner(Spawner spawner)
    {
        spawners.Add(spawner);
    }

    public void InitializeLevelOnline(PlayerSpawnpoint playerSpawnpoint)
    {
        if (CustomNetworkManager.Instance.IsServer && !hasInitializedLevel)
        {
            if (PlayerNetworkCharacter.LocalPlayer != null)
                NetworkServer.Destroy(PlayerNetworkCharacter.LocalPlayer.gameObject);

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
            PlayerNetworkIdentity.LocalPlayerInstance.Ready = false;
            PlayerNetworkIdentity.OtherPlayerInstance.Ready = false;

            //spawn
            foreach (Spawner spawner in spawners)
            {
                spawner.SpawnObject();
            }

            PlayerCharacters.Add(hostNetworkCharacter.PlayerMovement);
            PlayerCharacters.Add(clientNetworkCharacter.PlayerMovement);
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
            int totalCoinsEarned = 0;
            int totalJellyBeansEarned = 0;

            foreach (PlayerMovement playerMovement in PlayerCharacters)
            {
                totalCoinsEarned += playerMovement.Player.Coins;
                totalJellyBeansEarned += playerMovement.Player.JellyBeans;
            }

            int totalXpEarned = (totalCoinsEarned + totalJellyBeansEarned) * 2;

            if (CustomNetworkManager.HasAuthority)
                this.CallWithDelay(() => { ShowWinScreen(totalCoinsEarned, totalJellyBeansEarned, totalXpEarned); }, 1.5f);
        }
    }

    private void ShowWinScreen(int totalCoinsEarned, int totalJellyBeansEarned, int totalXpEarned)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
                NetworkMethodCaller.Instance.ShowWinScreen(totalCoinsEarned, totalJellyBeansEarned, totalXpEarned);
        }
        else
        {
            GameUi.Instance.ShowWinScreen(totalCoinsEarned, totalJellyBeansEarned, totalXpEarned);
        }
    }

    public void ContinueFromLevel()
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
        {
            NetworkMethodCaller.Instance.CmdContinueFromLevel();
            return;
        }

        if (GameStateManager.State == GameState.Story)
        {
            if (CustomNetworkManager.IsOnlineSession)
            {
                NetworkMethodCaller.Instance.GoToNextStoryLevel();
            }
            else
            {
                LoadNextLevel();
            }
        }
        else
        {
            if (!CustomNetworkManager.IsOnlineSession)
            {
                GameUi.Instance.ExitLevel();
            }
            else
            {
                NetworkMethodCaller.Instance.ExitLevel();
            }
        }
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        WorldBuilder.NextLevel = World.FromWorldName(currentLevel.ToString().PadLeft(2, '0'));
        LoadGamePlay();
    }

    private void LoadGamePlay()
    {
        if (Paused)
            GameUi.Instance.PauseMenuWasClosed();
        if (GameUi.Instance.WinScreen.gameObject.activeSelf)
            GameUi.Instance.CloseWinScreen();

        SceneManager.LoadScene("GamePlay");
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (GameStateManager.State == GameState.Free || GameStateManager.State == GameState.Story)
        {
            if (GameUi.Instance.WinScreen.gameObject.activeSelf) //can't pause while win screen is active
                return;
        }

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
