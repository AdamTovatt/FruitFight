using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool ShouldStartLevel;

    public List<PlayerConfiguration> Users;
    public List<PlayerMovement> PlayerCharacters;
    public bool IsDebug = false;
    public GameObject CameraPrefab;
    public GameObject PlayerPrefab;

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
        WorldBuilder.Instance.Build("01");
        navMeshSurface.BuildNavMesh();

        GameObject cameraObject = Instantiate(CameraPrefab, transform.position, transform.rotation);
        WorldBuilder.Instance.AddPreviousWorldObjects(cameraObject);
        GameObject.FindObjectOfType<SkyboxCamera>().SetMainCamera(cameraObject.transform);
        MultipleTargetCamera = cameraObject.GetComponent<MultipleTargetCamera>();

        PlayerSpawnpoint playerSpawnpoint = FindObjectOfType<PlayerSpawnpoint>();

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

    public void Update()
    {
        if (oldIsDebug != IsDebug)
        {
            OnDebugStateChanged?.Invoke(this, IsDebug);
        }

        oldIsDebug = IsDebug;
    }
}
