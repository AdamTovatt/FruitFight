using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(BlockThumbnailManager))]
public class WorldEditor : MonoBehaviour
{
    public static WorldEditor Instance { get; private set; }

    public GameObject GridLinePrefab;
    public GameObject MainCameraPrefab;
    public GameObject MarkerPrefab;

    public WorldEditorUi Ui;

    public float CameraSmoothTime = 1f;
    public float MarkerMoveCooldown = 0.5f;
    public float MarkerMoveSensitivity = 0.5f;

    public WorldBuilder Builder { get; private set; }
    public World CurrentWorld { get; private set; }

    public BlockThumbnailManager ThumbnailManager { get; private set; }

    public int GridSize { get; set; }
    public int SelectedBlock { get { return selectedBlock; } set { SetSelectedBlock(value); } }
    private int selectedBlock;

    public Vector3Int MarkerPosition { get { return new Vector3Int(marker.transform.position); } }

    private List<GameObject> gridLines;
    private MultipleTargetCamera mainCamera;
    private EditorMarker marker;
    private bool controlsDisabled = false;
    private bool isTestingLevel = false;

    private float lastMarkerMoveTime;

    private PlayerControls input;

    public void Awake()
    {
        Instance = this;

        ThumbnailManager = gameObject.GetComponent<BlockThumbnailManager>();

        GridSize = 4;
        SelectedBlock = 1;
        lastMarkerMoveTime = Time.time - MarkerMoveCooldown * 1.2f;

        gridLines = new List<GameObject>();

        if (CurrentWorld == null)
        {
            CurrentWorld = new World();
            CurrentWorld.Add(new Block(BlockInfoLookup.Get(3), new Vector3Int(0, -3, 0)));
        }

        Builder = gameObject.GetComponent<WorldBuilder>();
        WorldBuilder.IsInEditor = true;

        mainCamera = Instantiate(MainCameraPrefab).GetComponent<MultipleTargetCamera>();

        input = new PlayerControls();
        input.LevelEditor.Enable();
        input.LevelEditor.Place.performed += Place;
        input.LevelEditor.MoveMarker.performed += MoveMarker;
        input.LevelEditor.RaiseMarker.performed += RaiseMarker;
        input.LevelEditor.LowerMarker.performed += LowerMarker;
        input.LevelEditor.MoveMarker.canceled += MoveMarkerCanceled;
        input.LevelEditor.Pause.performed += Pause;
        input.LevelEditor.MoveBlockSelection.performed += MoveBlockSelection;
    }

    void Start()
    {
        Debug.Log("world editor start");
        marker = Instantiate(MarkerPrefab).GetComponent<EditorMarker>();
        CreateGridFromMarker();

        GetComponent<Spawner>().OnObjectSpawned += SpawnerSpawnedObject;

        mainCamera.Offset = mainCamera.Offset * 2;
        mainCamera.SmoothTime = CameraSmoothTime;
        mainCamera.Targets.Add(marker.transform);

        Ui = FindObjectOfType<WorldEditorUi>();
    }

    private void SpawnerSpawnedObject(object sender, GameObject spawnedObject)
    {
        spawnedObject.GetComponentInChildren<SkyboxCamera>().SetMainCamera(mainCamera.transform);
        GetComponent<Spawner>().OnObjectSpawned -= SpawnerSpawnedObject;
    }

    public void TestLevelButton()
    {
        if (CurrentWorld.Blocks.Where(x => x.BlockInfoId == 2).Count() < 1)
        {
            Alert alert = WorldEditorUi.Instance.AlertCreator.CreateAlert("No player start point exists in the level", new List<string>() { "Go back", "Continue anyway" });
            alert.OnOptionWasChosen += (sender, optionIndex) =>
            {
                if (optionIndex == 0)
                    WorldEditorUi.Instance.ClosePauseMenu();
                else if (optionIndex == 1)
                    StartLevelTest();
            };
        }
        else
        {
            StartLevelTest();
        }
    }

    public void SetSelectedBlock(int newValue)
    {
        if (newValue == selectedBlock)
            return;

        BlockInfo block = BlockInfoLookup.Get(newValue);
        GridSize = block.Width;
        selectedBlock = newValue;

        if (marker != null)
        {
            CreateGridFromMarker();
            float scale = (float)GridSize / 4;
            marker.GetComponentInChildren<SizeSine>().BaseScale = scale;
            marker.transform.position = SetOnGrid(MarkerPosition);
            marker.GetComponent<EditorMarker>().SetMarkerSize(new Vector2Int(block.Width, block.Width));
        }
    }

    private void StartLevelTest()
    {
        Ui.ShowLoadingScreen();
        Ui.ClosePauseMenu();
        Ui.HideBlockSelection();
        WorldBuilder.NextLevel = CurrentWorld;
        SceneManager.LoadScene("PlayerSetup");
        SceneManager.sceneLoaded += LevelTestWasLoaded;
        isTestingLevel = true;
        controlsDisabled = false;
        Destroy(mainCamera);
    }

    private void LevelTestWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        GameManager.ShouldStartLevel = true;
        Ui.HideLoadingScreen();
        SceneManager.sceneLoaded -= LevelTestWasLoaded;
    }

    private void LevelEditorWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        WorldEditorUi.Instance.HideLoadingScreen();
        WorldEditorUi.Instance.ShowBlockSelection();
        SceneManager.sceneLoaded -= LevelEditorWasLoaded;
        input.LevelEditor.Place.performed -= Place;
        input.LevelEditor.MoveMarker.performed -= MoveMarker;
        input.LevelEditor.RaiseMarker.performed -= RaiseMarker;
        input.LevelEditor.LowerMarker.performed -= LowerMarker;
        input.LevelEditor.MoveMarker.canceled -= MoveMarkerCanceled;
        input.LevelEditor.Pause.performed -= Pause;
        input.LevelEditor.MoveBlockSelection.performed -= MoveBlockSelection;

        WorldBuilder.Instance.BuildWorld(CurrentWorld);
        Instance.CurrentWorld = CurrentWorld;
    }

    public void ExitLevelTest()
    {
        Ui.ShowLoadingScreen();
        isTestingLevel = false;
        controlsDisabled = true;

        foreach (PlayerMovement player in GameManager.Instance.PlayerCharacters)
        {
            Destroy(player.gameObject);
        }

        Destroy(GameManager.Instance.gameObject);
        Destroy(PlayerConfigurationManager.Instance.gameObject);

        SceneManager.LoadScene("LevelEditor");
        SceneManager.sceneLoaded += LevelEditorWasLoaded;
    }

    private void Pause(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!isTestingLevel)
        {
            if (!controlsDisabled)
            {
                controlsDisabled = true;
                Ui.OpenPauseMenu();
            }
            else
            {
                controlsDisabled = false;
                Ui.ClosePauseMenu();
            }
        }
        else
        {
            if (!controlsDisabled)
            {
                controlsDisabled = true;
                Ui.OpenLevelTestPauseMenu();
            }
            else
            {
                controlsDisabled = false;
                Ui.CloseLevelTestPauseMenu();
            }
        }
    }

    public void EnableControls()
    {
        controlsDisabled = false;
    }

    private void MoveBlockSelection(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!controlsDisabled || isTestingLevel)
        {
            Vector2 rawMoveValue = context.ReadValue<Vector2>();

            int binaryX = Mathf.Abs(rawMoveValue.x) > 0 ? (Mathf.Abs(rawMoveValue.x) < 1 ? 0 : (int)rawMoveValue.x) : 0;
            int binaryY = Mathf.Abs(rawMoveValue.y) > 0 ? (Mathf.Abs(rawMoveValue.y) < 1 ? 0 : (int)rawMoveValue.y) : 0;

            if (!(Mathf.Abs(binaryY) < 1 && Mathf.Abs(binaryX) < 1))
                Ui.BlockMenu.MoveBlockButtonSelection(new Vector2Int(binaryX, binaryY));
        }
    }

    private void CloseBlockSelection()
    {
        if (Ui.BlockMenu.IsOpen)
        {
            Ui.BlockMenu.Close();
        }
    }

    private void LowerMarker(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        CloseBlockSelection();

        marker.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y - GridSize, marker.transform.position.z);
        CreateGridFromMarker();
        lastMarkerMoveTime = Time.time;
    }

    private void RaiseMarker(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        CloseBlockSelection();

        marker.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y + GridSize, marker.transform.position.z);
        CreateGridFromMarker();
        lastMarkerMoveTime = Time.time;
    }

    private void Place(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        CloseBlockSelection();

        Block block = new Block(BlockInfoLookup.Get(SelectedBlock), MarkerPosition);
        CurrentWorld.Add(block);
        CurrentWorld.CalculateNeighbors();

        Builder.BuildWorld(CurrentWorld);
    }

    private void MoveMarkerCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        lastMarkerMoveTime = Time.time - MarkerMoveCooldown * 1.2f;
    }

    private void MoveMarker(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (marker == null)
        {
            Destroy(this);
            return;
        }

        if (controlsDisabled || isTestingLevel)
            return;

        if (Time.time - lastMarkerMoveTime >= MarkerMoveCooldown)
        {
            Vector2 input = context.ReadValue<Vector2>();

            int x = Mathf.Abs(input.x) > MarkerMoveSensitivity ? (input.x > 0 ? 1 : -1) : 0;
            int y = Mathf.Abs(input.y) > MarkerMoveSensitivity ? (input.y > 0 ? 1 : -1) : 0;

            if (x != 0 || y != 0)
            {
                marker.transform.position = new Vector3(marker.transform.position.x + (x * GridSize), marker.transform.position.y, marker.transform.position.z + (y * GridSize));
                CreateGridFromMarker();
                lastMarkerMoveTime = Time.time;
            }

            CloseBlockSelection();
        }
    }

    private Vector3 SetOnGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x / GridSize) * GridSize, Mathf.Round(position.y / GridSize) * GridSize, Mathf.Round(position.z / GridSize) * GridSize);
    }

    private void CreateGridFromMarker()
    {
        CreateGrid(4, new Vector3(marker.transform.position.x, marker.transform.position.y - GridSize, marker.transform.position.z));
    }

    private void CreateGrid(int tiles, Vector3 centerPoint)
    {
        foreach (GameObject grid in gridLines)
        {
            Destroy(grid);
        }

        for (int x = -tiles; x < tiles + 1; x++)
        {
            for (int z = -tiles; z < tiles + 1; z++)
            {
                Vector3 currentPosition = new Vector3(centerPoint.x + x * GridSize, centerPoint.y, centerPoint.z + z * GridSize);
                Vector3 nextPositionX = new Vector3(centerPoint.x + (x + 1) * GridSize, centerPoint.y, centerPoint.z + z * GridSize);
                Vector3 nextPositionZ = new Vector3(centerPoint.x + x * GridSize, centerPoint.y, centerPoint.z + (z + 1) * GridSize);

                if (x < tiles)
                {
                    GameObject gridLineX = Instantiate(GridLinePrefab, transform);
                    gridLines.Add(gridLineX);

                    LineRenderer lineX = gridLineX.GetComponent<LineRenderer>();
                    lineX.SetPosition(0, currentPosition);
                    lineX.SetPosition(1, nextPositionX);
                }

                if (z < tiles)
                {
                    GameObject gridLineZ = Instantiate(GridLinePrefab, transform);
                    gridLines.Add(gridLineZ);

                    LineRenderer lineZ = gridLineZ.GetComponent<LineRenderer>();
                    lineZ.SetPosition(0, currentPosition);
                    lineZ.SetPosition(1, nextPositionZ);
                }
            }
        }
    }
}
