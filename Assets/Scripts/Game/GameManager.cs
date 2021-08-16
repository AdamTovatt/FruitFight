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
    private Spawner skyboxSpawner;
    private static float currentLevel = 1;

    public void Awake()
    {
        Players = new List<PlayerInformation>();
        PlayerCharacters = new List<PlayerMovement>();
        navMeshSurface = GetComponent<NavMeshSurface>();
        skyboxSpawner = GetComponent<Spawner>();

        skyboxSpawner.OnObjectSpawned += SkyboxWasCreated;
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
        playerControls.Gameplay.Pause.performed += Pause;
    }

    private void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.Gameplay.Pause.performed -= Pause;
        }

        skyboxSpawner.OnObjectSpawned -= SkyboxWasCreated;
    }

    private void SkyboxWasCreated(object sender, GameObject spawnedObject)
    {
        if (MultipleTargetCamera != null)
            spawnedObject.GetComponentInChildren<SkyboxCamera>().SetMainCamera(MultipleTargetCamera.transform);
    }

    public void StartLevel()
    {
        WorldBuilder.IsInEditor = false;
        WorldBuilder.Instance.BuildNext();
        navMeshSurface.BuildNavMesh();

        GameObject cameraObject = Instantiate(CameraPrefab, transform.position, transform.rotation);
        WorldBuilder.Instance.AddPreviousWorldObjects(cameraObject);
        MultipleTargetCamera = cameraObject.GetComponent<MultipleTargetCamera>();
        MultipleTargetCamera.SetCameraHints(FindObjectsOfType<CameraHint>());

        PlayerSpawnpoint playerSpawnpoint = FindObjectOfType<PlayerSpawnpoint>();

        if (playerSpawnpoint != null)
        {
            foreach (PlayerConfiguration playerConfiguration in PlayerConfigurationManager.Instance.PlayerConfigurations.ToArray())
            {
                playerConfiguration.Input.SwitchCurrentActionMap("Gameplay");

                GameObject player = Instantiate(PlayerPrefab, playerSpawnpoint.transform.position, playerSpawnpoint.transform.rotation);
                PlayerMovement playerMovement = player.gameObject.GetComponent<PlayerMovement>();
                MultipleTargetCamera.Targets.Add(player.transform);

                GameObject hatPrefab = PrefabKeeper.Instance.GetPrefab(playerConfiguration.GetHatAsPrefabEnum());
                if (hatPrefab != null)
                {
                    GameObject playerHat = Instantiate(hatPrefab, playerMovement.GetComponentInChildren<HatPoint>().transform.position, playerMovement.transform.rotation);
                    playerHat.transform.SetParent(playerMovement.transform.GetComponentInChildren<HatPoint>().transform);
                }

                playerMovement.InitializePlayerInput(playerConfiguration);
                PlayerCharacters.Add(playerMovement);

                Players.Add(new PlayerInformation(playerConfiguration, playerMovement, playerMovement.gameObject.GetComponent<Health>()));
            }
        }
        else
        {
            MultipleTargetCamera.Targets.Add(Instantiate(new GameObject(), new Vector3(0, 3, 0), Quaternion.identity, transform).transform);
        }

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

        World currentWorld = WorldBuilder.Instance.CurrentWorld;
        HashSet<Block> blocks = new HashSet<Block>();
        foreach (PlayerMovement player in PlayerCharacters)
        {
            Ray ray = new Ray(MultipleTargetCamera.transform.position, (player.transform.position - MultipleTargetCamera.transform.position));
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.transform == player.transform)
                    continue;

                if (hitInfo.collider.bounds.size.sqrMagnitude < player.Collider.bounds.size.sqrMagnitude)
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
        if (WorldEditor.Instance == null || !WorldEditor.Instance.IsTestingLevel)
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
        if (WorldEditorUi.Instance == null) //if we come from the world editor we will let the world editor handle the ui
        {
            if (!Paused)
            {
                DisablePlayerControls();
                Paused = true;
            }
            else
            {
                EnablePlayerControls();
                Paused = false;
            }
        }
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
    }

    public void EnablePlayerControls()
    {
        SetPlayerControls(true);
    }
}
