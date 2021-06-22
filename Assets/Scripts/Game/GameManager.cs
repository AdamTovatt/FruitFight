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

    public List<PlayerConfiguration> Users;
    public List<PlayerMovement> PlayerCharacters;
    public bool IsDebug = false;
    public GameObject CameraPrefab;
    public GameObject PlayerPrefab;
    public int BlockSeeThroughRadius = 2;

    public MultipleTargetCamera MultipleTargetCamera { get; private set; }

    public delegate void IsDebugChangedHandler(object sender, bool newState);
    public event IsDebugChangedHandler OnDebugStateChanged;

    private bool oldIsDebug = false;
    private PlayerInputManager playerInputManager;
    private NavMeshSurface navMeshSurface;

    public void Awake()
    {
        Users = new List<PlayerConfiguration>();
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
    }

    public void StartLevel()
    {
        WorldBuilder.IsInEditor = false;
        WorldBuilder.Instance.BuildNext();
        navMeshSurface.BuildNavMesh();

        GameObject cameraObject = Instantiate(CameraPrefab, transform.position, transform.rotation);
        WorldBuilder.Instance.AddPreviousWorldObjects(cameraObject);
        FindObjectOfType<SkyboxCamera>().SetMainCamera(cameraObject.transform);
        MultipleTargetCamera = cameraObject.GetComponent<MultipleTargetCamera>();

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
            }
        }
        else
        {
            MultipleTargetCamera.Targets.Add(Instantiate(new GameObject(), new Vector3(0, 3, 0), Quaternion.identity, transform).transform);
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
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.transform == player.transform)
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
            if(block.SeeThroughBlock != null)
            {
                block.SeeThroughBlock.Enable();
            }
        }
    }
}
