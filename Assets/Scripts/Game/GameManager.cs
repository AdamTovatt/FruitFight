using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Linq;
using Mirror;
using System;

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

    private bool hasInitializedLevel;

    private List<Spawner> spawners = new List<Spawner>();
    private Dictionary<Transform, BlockInformationHolder> blockInformationDictionary = new Dictionary<Transform, BlockInformationHolder>();

    private DateTime levelStartTime;

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
        levelStartTime = DateTime.Now;
        Debug.Log("player characters count: " + PlayerCharacters.Count);

        if (CustomNetworkManager.IsOnlineSession && NetworkMethodCaller.Instance != null)
            NetworkMethodCaller.Instance.ClearBouncyObjects(); //clear the dictionary with bouncy objects so it can be filled again with the ones from this level

        TransformsWithHealth.Clear();

        WorldBuilder.IsInEditor = false;
        WorldBuilder.Instance.BuildNext();
        navMeshSurface.BuildNavMesh();

        PlayerSpawnpoint[] playerSpawnpoints = FindObjectsOfType<PlayerSpawnpoint>();

        bool addedPlayerCamera = false;

        if (playerSpawnpoints.Length > 0)
        {
            if (!CustomNetworkManager.IsOnlineSession) //only if this is an offline session
            {
                PlayerSpawnpoint spawnPoint = playerSpawnpoints[0];

                PlayerConfiguration[] playerConfigurations = PlayerConfigurationManager.Instance.PlayerConfigurations.ToArray();
                foreach (PlayerConfiguration playerConfiguration in playerConfigurations)
                {
                    playerConfiguration.Input.SwitchCurrentActionMap("Gameplay");

                    GameObject player = Instantiate(PlayerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    PlayerMovement playerMovement = player.gameObject.GetComponent<PlayerMovement>();

                    playerMovement.Player.PlayerNumber = addedPlayerCamera ? 2 : 1; //set up player number to displaye player indicator correctly
                    playerMovement.Player.SetupPlayerIndicator();

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

                    if (playerSpawnpoints.Length > 1)
                    {
                        spawnPoint = playerSpawnpoints[1];
                    }
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

        HandleStoryMode();

        GameUi.Instance.HideLoadingScreen();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleStoryMode()
    {
        if (GameStateManager.State == GameState.Story)
        {
            if (!CustomNetworkManager.IsOnlineSession || hasInitializedLevel)
            {
                ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();

                PlayerCharacters[0].Player.RemoveItem(AbsorbableItemType.Coin, -save.Coins); //remove negative amount to add it
                PlayerCharacters[0].Player.RemoveItem(AbsorbableItemType.JellyBean, -save.JellyBeans);
            }
        }
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
            InitializeLevelOnline(FindObjectsOfType<PlayerSpawnpoint>());
        }
    }

    public void AddSpawner(Spawner spawner)
    {
        spawners.Add(spawner);
    }

    public void InitializeLevelOnline(PlayerSpawnpoint[] playerSpawnpoints)
    {
        if (CustomNetworkManager.Instance.IsServer && !hasInitializedLevel)
        {
            PlayerSpawnpoint spawnPoint1 = playerSpawnpoints[0];
            PlayerSpawnpoint spawnPoint2 = spawnPoint1;
            if (playerSpawnpoints.Length > 1)
            {
                spawnPoint2 = playerSpawnpoints[1];
            }

            if (PlayerNetworkCharacter.LocalPlayer != null)
                NetworkServer.Destroy(PlayerNetworkCharacter.LocalPlayer.gameObject);

            //host stuff
            GameObject hostPlayer = Instantiate(PlayerPrefab, Vector3.up * 0.5f + spawnPoint1.transform.position + spawnPoint1.transform.right * 0.2f, spawnPoint1.transform.rotation);
            NetworkServer.Spawn(hostPlayer);
            PlayerNetworkCharacter hostNetworkCharacter = hostPlayer.GetComponent<PlayerNetworkCharacter>();
            hostNetworkCharacter.NetId = PlayerNetworkIdentity.LocalPlayerInstance.NetId;

            hostNetworkCharacter.SetupPlayerNetworkCharacter(true);

            //client stuff
            GameObject clientPlayer = Instantiate(PlayerPrefab, Vector3.up * 0.5f + spawnPoint2.transform.position + spawnPoint2.transform.right * -0.2f, spawnPoint2.transform.rotation);
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

            hostNetworkCharacter.AddSelfToPlayerCharacters();
            clientNetworkCharacter.AddSelfToPlayerCharacters();
            Debug.Log("add to characters: " + PlayerCharacters.Count);

            HandleStoryMode();
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
            int totalCoins = 0;
            int totalJellyBeans = 0;

            PlayerCharacters = PlayerCharacters.Distinct().ToList();

            foreach (PlayerMovement playerMovement in PlayerCharacters)
            {
                totalCoins += playerMovement.Player.Coins;
                totalJellyBeans += playerMovement.Player.JellyBeans;
            }

            if (GameStateManager.State == GameState.Story)
            {
                ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();

                if (save != null)
                {
                    totalCoins -= save.Coins;
                    totalJellyBeans -= save.JellyBeans;
                }
            }

            int totalXpEarned = (totalCoins + totalJellyBeans * 2) * 2;

            if (CustomNetworkManager.HasAuthority)
            {
                if (GameStateManager.State == GameState.Story)
                {
                    ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();
                    if (save != null)
                    {
                        save.AddCompletedLevel(WorldBuilder.Instance.CurrentWorld.StoryModeLevelEntry.Id);
                        save.Coins += totalCoins;
                        save.JellyBeans += totalJellyBeans;
                        save.Xp += totalXpEarned;
                        save.HoursPlayed += (DateTime.Now - levelStartTime).TotalHours;
                        SaveProfileHelper.WriteSaveState(SaveProfileHelper.GetSaveState());
                    }
                }

                this.CallWithDelay(() => { ShowWinScreen(totalCoins, totalJellyBeans, totalXpEarned); }, 1.5f);
            }
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

    private void AddLevelAsCompleted()
    {
        ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();
        save.AddCompletedLevel(WorldBuilder.Instance.CurrentWorld.StoryModeLevelEntry.Id);
        SaveProfileHelper.WriteSaveState(SaveProfileHelper.GetSaveState());
    }

    public void ContinueFromLevel()
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
        {
            NetworkMethodCaller.Instance.CmdContinueFromLevel();
            return;
        }

        World nextWorld = GetNextStoryWorld();

        if (GameStateManager.State == GameState.Story)
        {
            if (nextWorld != null)
            {
                LoadNextStoryLevel(nextWorld.Metadata.Name);
            }
            else
            {
                ExitLevel();
            }
        }
        else
        {
            ExitLevel();
        }
    }

    private void ExitLevel()
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

    private World GetNextStoryWorld()
    {
        int nextWorldId = WorldBuilder.Instance.CurrentWorld.StoryModeLevelEntry.Id + 1;
        ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();

        return WorldUtilities.GetStoryModeLevels().Find(x => x.StoryModeLevelEntry.Id == nextWorldId);
    }

    public void LoadNextStoryLevel(string name)
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.HasAuthority)
            return;

        ProfileSave save = SaveProfileHelper.GetCurrentSaveProfile();
        World nextWorld = WorldUtilities.GetStoryModeLevel(name);

        if (save.HasUnlockedLevel(nextWorld.StoryModeLevelEntry, out string _buttonText))
        {
            if (!CustomNetworkManager.IsOnlineSession)
            {
                WorldBuilder.NextLevel = nextWorld;
                LoadGamePlay();
            }
            else
            {
                if (CustomNetworkManager.HasAuthority)
                {
                    Debug.Log("call rpc load gameplay");
                    NetworkMethodCaller.Instance.RpcLoadGameplay(name);
                }
            }
        }
        else
        {
            ExitLevel();
        }
    }

    public void LoadGamePlay()
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
            if (GameUi.Instance.WinScreen != null && GameUi.Instance.WinScreen.gameObject.activeSelf) //can't pause while win screen is active
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

    public BlockInformationHolder GetBlockInformationHolder(Transform transform)
    {
        if (!blockInformationDictionary.ContainsKey(transform))
            blockInformationDictionary.Add(transform, transform.GetComponentInParent<BlockInformationHolder>());

        return blockInformationDictionary[transform];
    }
}
