using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Linq;

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

    public void Awake()
    {
        Players = new List<PlayerInformation>();
        PlayerCharacters = new List<PlayerMovement>();
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void Start()
    {
        if (PlayerConfigurationManager.Instance == null)
        {
            SceneManager.LoadScene("PlayerSetup");
            Destroy(gameObject);
            return;
        }

        if (Instance != null)
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
        }
        else
        {
            CameraManager.AddCamera(transform, null, false);
        }

        foreach (PlayerInformation player in Players)
        {
            GameUi.Instance.CreatePlayerInfoUi(player);
            player.Movement.transform.parent = transform;
        }

        GameUi.Instance.HideLoadingScreen();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (oldIsDebug != IsDebug)
        {
            OnDebugStateChanged?.Invoke(this, IsDebug);
        }

        oldIsDebug = IsDebug;

        World currentWorld = WorldBuilder.Instance.CurrentWorld;
        HashSet<Block> blocks = new HashSet<Block>();
        foreach (SingleTargetCamera camera in CameraManager.Cameras)
        {
            foreach (PlayerMovement player in PlayerCharacters)
            {
                Ray ray = new Ray(camera.transform.position, ((player.transform.position + Vector3.up * 0.2f) - camera.transform.position));
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    if (hitInfo.transform == player.transform || hitInfo.transform.tag == "Invisible")
                        continue;

                    if (hitInfo.collider.bounds.size.sqrMagnitude < player.Collider.bounds.size.sqrMagnitude * 3)
                        continue;
                }

                for (int x = 0; x < BlockSeeThroughRadius * 2; x++)
                {
                    for (int y = 0; y < BlockSeeThroughRadius * 2; y++)
                    {
                        for (int z = 0; z < BlockSeeThroughRadius * 2; z++)
                        {
                            Vector3Int searchPosition = ((Vector3Int)player.transform.position) + new Vector3Int(BlockSeeThroughRadius - x, BlockSeeThroughRadius - y, BlockSeeThroughRadius - z);

                            foreach (Block block in currentWorld.GetBlocksAtPosition(searchPosition))
                            {
                                blocks.Add(block);
                            }
                        }
                    }
                }
            }
        }

        foreach (Block block in blocks)
        {
            if (block.SeeThroughBlock != null)
            {
                block.SeeThroughBlock.Enable();
            }
        }
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
