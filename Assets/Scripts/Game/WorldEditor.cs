using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(BlockThumbnailManager))]
public class WorldEditor : MonoBehaviour
{
    public static WorldEditor Instance { get; private set; }

    public GameObject GridLinePrefab;
    public GameObject CameraPrefab;
    public GameObject MarkerPrefab;

    public WorldEditorUi Ui;

    public float CameraSmoothTime = 1f;
    public float MarkerMoveCooldown = 0.5f;
    public float MarkerMoveSensitivity = 0.5f;
    public float ObjectRotationSpeed = 180f;

    public WorldBuilder Builder { get; private set; }
    public World CurrentWorld { get; private set; }

    public BlockThumbnailManager ThumbnailManager { get; private set; }

    public int GridSize { get; set; }
    public int SelectedBlock { get { return selectedBlock; } set { SetSelectedBlock(value); } } //this is the selected block in the blockselection ui menu. Is the block info id
    private int selectedBlock;

    public Vector3Int MarkerPosition { get { return new Vector3Int(marker.transform.position); } }
    public bool IsTestingLevel { get { return isTestingLevel; } }

    private List<GameObject> gridLines;
    private EditorCamera editorCamera;
    private EditorMarker marker;
    private bool controlsDisabled = false;
    private bool isTestingLevel = false;
    private bool isRotatingObject = false;
    private bool isPickingActivator = false;
    private bool isPickingFinalPosition = false;
    private Vector2 currentLeftStickInput;
    private Block selectedWorldObject; //this is the selected object in the world, the object which the marker is above
    private MoveMenu moveMenu;

    private float lastMarkerMoveTime;
    private float lastPageSwitchTime;

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

        editorCamera = Instantiate(CameraPrefab).GetComponent<EditorCamera>();

        input = new PlayerControls();
        input.LevelEditor.Enable();
        input.LevelEditor.Place.performed += Place;
        input.LevelEditor.MoveMarker.performed += MoveMarker;
        input.LevelEditor.MoveMarker.canceled += MoveMarkerCanceled;
        input.LevelEditor.RaiseMarker.performed += RaiseMarker;
        input.LevelEditor.LowerMarker.performed += LowerMarker;
        input.LevelEditor.MoveMarker.canceled += MoveMarkerCanceled;
        input.LevelEditor.Pause.performed += Pause;
        input.LevelEditor.MoveBlockSelection.performed += MoveBlockSelection;
        input.LevelEditor.NextPage.performed += NextPage;
        input.LevelEditor.PreviousPage.performed += PreviousPage;
        input.LevelEditor.NextPage.canceled += CancelNextPage;
        input.LevelEditor.PreviousPage.canceled += CancelPreviousPage;
        input.LevelEditor.Remove.performed += Remove;
        input.LevelEditor.Rotate.performed += editorCamera.Rotate;
        input.LevelEditor.Rotate.canceled += editorCamera.CancelRotate;
        input.LevelEditor.ToggleObjectRotation.performed += ToggleRotateObject;
    }

    void Start()
    {
        Debug.Log("world editor start");
        marker = Instantiate(MarkerPrefab).GetComponent<EditorMarker>();
        CreateGridFromMarker();

        GetComponent<Spawner>().OnObjectSpawned += SpawnerSpawnedObject;

        editorCamera.Target = marker.transform;

        Ui = FindObjectOfType<WorldEditorUi>();
    }

    private void Update()
    {
        if (isRotatingObject)
        {
            float x = selectedWorldObject.Info.RotatableX ? currentLeftStickInput.x : 0;
            float y = selectedWorldObject.Info.RotatableY ? currentLeftStickInput.y : 0;
            selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(0, x, 0), Time.deltaTime * ObjectRotationSpeed * Mathf.Abs(x));
            selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(y, 0, 0), Time.deltaTime * ObjectRotationSpeed * Mathf.Abs(y));
        }
    }

    private void SpawnerSpawnedObject(object sender, GameObject spawnedObject)
    {
        spawnedObject.GetComponentInChildren<SkyboxCamera>().SetMainCamera(editorCamera.transform);
        GetComponent<Spawner>().OnObjectSpawned -= SpawnerSpawnedObject;
    }

    public void PickMoveFinalPosition(MoveMenu menu, Block block)
    {
        menu.gameObject.SetActive(false);
        moveMenu = menu;
        WorldEditorUi.Instance.BehaviourMenu.gameObject.SetActive(false);
        WorldEditorUi.Instance.DisableUiInput();
        EnableControls();
        isPickingFinalPosition = true;
        SetSelectedBlock(block.BlockInfoId);
    }

    public void PickedMoveFinalPosition(Vector3Int position)
    {
        WorldEditorUi.Instance.BehaviourMenu.gameObject.SetActive(true);
        WorldEditorUi.Instance.EnableUiInput();
        controlsDisabled = true;
        isPickingFinalPosition = false;
        moveMenu.gameObject.SetActive(true);
        moveMenu.FinalPositionWasSet(position);
    }

    public void PickActivator(MoveMenu menu)
    {
        menu.gameObject.SetActive(false);
        moveMenu = menu;
        WorldEditorUi.Instance.BehaviourMenu.gameObject.SetActive(false);
        WorldEditorUi.Instance.DisableUiInput();
        EnableControls();
        isPickingActivator = true;
    }

    private void PickedActivator(Block selectedStateSwitcher)
    {
        WorldEditorUi.Instance.BehaviourMenu.gameObject.SetActive(true);
        WorldEditorUi.Instance.EnableUiInput();
        controlsDisabled = true;
        isPickingActivator = false;
        moveMenu.gameObject.SetActive(true);
        moveMenu.ActivatorWasSet(selectedStateSwitcher);
    }

    public void SaveLevel()
    {
        Ui.AlertCreator.CreateAlert("Enter level name to save the level (will overwrite)").OnOptionWasChosen += (sender, optionIndex) =>
        {
            Ui.OnScreenKeyboard.OpenKeyboard();
            Ui.OnScreenKeyboard.OnGotText += (sender, success, text) =>
            {
                if (success)
                {
                    if (CurrentWorld.Metadata == null)
                        CurrentWorld.Metadata = new WorldMetadata(text);

                    string mapDirectory = string.Format("{0}/maps", Application.persistentDataPath);

                    if (!Directory.Exists(mapDirectory))
                        Directory.CreateDirectory(mapDirectory);

                    string fileName = text.Replace(' ', '_');
                    string levelPath = string.Format("{0}/{1}.map", mapDirectory, fileName);
                    string metaPath = string.Format("{0}/{1}.meta", mapDirectory, fileName);

                    File.WriteAllText(levelPath, CurrentWorld.ToJson());
                    File.WriteAllText(metaPath, CurrentWorld.Metadata.ToJson());

                    Debug.Log("Wrote map data to: " + levelPath);
                }

                Ui.ClosePauseMenu();
            };
        };
    }

    public void LoadLevel()
    {
        Ui.AlertCreator.CreateAlert("Enter level name to load a level").OnOptionWasChosen += (sender, optionIndex) =>
        {
            Ui.OnScreenKeyboard.OpenKeyboard();
            Ui.OnScreenKeyboard.OnGotText += (sender, success, text) =>
            {
                if (success)
                {
                    string mapDirectory = string.Format("{0}/maps", Application.persistentDataPath);

                    if (!Directory.Exists(mapDirectory))
                        Directory.CreateDirectory(mapDirectory);

                    string fileName = text.Replace(' ', '_');
                    string levelPath = string.Format("{0}/{1}.map", mapDirectory, fileName);
                    string metaPath = string.Format("{0}/{1}.meta", mapDirectory, fileName);

                    if (File.Exists(levelPath) && File.Exists(metaPath))
                    {
                        CurrentWorld = World.FromJson(File.ReadAllText(levelPath));
                        Builder.BuildWorld(CurrentWorld);

                        Ui.ClosePauseMenu();
                    }
                    else
                    {
                        Ui.AlertCreator.CreateAlert(string.Format("No such level found:\n{0}", text)).OnOptionWasChosen += (o, i) => { Ui.ClosePauseMenu(); };
                    }
                }
            };
        };
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
        Destroy(editorCamera);
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
        input.LevelEditor.NextPage.performed -= NextPage;
        input.LevelEditor.PreviousPage.performed -= PreviousPage;
        input.LevelEditor.NextPage.canceled -= CancelNextPage;
        input.LevelEditor.PreviousPage.canceled -= CancelPreviousPage;
        input.LevelEditor.Remove.performed -= Remove;
        input.LevelEditor.Rotate.performed -= editorCamera.Rotate;
        input.LevelEditor.Rotate.canceled -= editorCamera.CancelRotate;
        input.LevelEditor.ToggleObjectRotation.performed -= ToggleRotateObject;

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

    private void Pause(InputAction.CallbackContext context)
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

    private void CancelNextPage(InputAction.CallbackContext context)
    {
        editorCamera.EndZoomIn();
    }

    private void CancelPreviousPage(InputAction.CallbackContext context)
    {
        editorCamera.EndZoomOut();
    }

    private void NextPage(InputAction.CallbackContext context)
    {
        if (!controlsDisabled || isTestingLevel)
        {
            if (Ui.BlockMenu.IsOpen)
            {
                if (Time.time - lastPageSwitchTime > 0.2f)
                {

                    Ui.BlockMenu.NextPage();

                    lastPageSwitchTime = Time.time;
                }
            }
            else
            {
                editorCamera.StartZoomIn();
            }
        }
    }

    private void PreviousPage(InputAction.CallbackContext context)
    {
        if (!controlsDisabled || isTestingLevel)
        {
            if (Ui.BlockMenu.IsOpen)
            {
                if (Time.time - lastPageSwitchTime > 0.2f)
                {
                    Ui.BlockMenu.PreviousPage();

                    lastPageSwitchTime = Time.time;
                }
            }
            else
            {
                editorCamera.StartZoomOut();
            }
        }
    }

    private void ToggleRotateObject(InputAction.CallbackContext context)
    {
        if (!isRotatingObject)
        {
            if (selectedWorldObject != null)
            {
                if (selectedWorldObject.Info.RotatableX || selectedWorldObject.Info.RotatableY)
                {
                    isRotatingObject = true;
                }
            }
        }
        else
        {
            ExitRotationMode();
        }
    }

    private void ExitRotationMode()
    {
        selectedWorldObject.Rotation = selectedWorldObject.Instance.transform.rotation;
        selectedWorldObject.RotationOffset = selectedWorldObject.Instance.transform.position - selectedWorldObject.Position;
        isRotatingObject = false;
    }

    private void MoveBlockSelection(InputAction.CallbackContext context)
    {
        if (!isPickingFinalPosition)
        {
            if (!controlsDisabled || isTestingLevel)
            {
                if (isRotatingObject)
                    ExitRotationMode();

                Vector2 rawMoveValue = context.ReadValue<Vector2>();
                currentLeftStickInput = rawMoveValue;

                int binaryX = Mathf.Abs(rawMoveValue.x) > 0 ? (Mathf.Abs(rawMoveValue.x) < 1 ? 0 : (int)rawMoveValue.x) : 0;
                int binaryY = Mathf.Abs(rawMoveValue.y) > 0 ? (Mathf.Abs(rawMoveValue.y) < 1 ? 0 : (int)rawMoveValue.y) : 0;

                if (!(Mathf.Abs(binaryY) < 1 && Mathf.Abs(binaryX) < 1))
                    Ui.BlockMenu.MoveBlockButtonSelection(new Vector2Int(binaryX, binaryY));
            }
        }
    }

    private void CloseBlockSelection()
    {
        if (Ui.BlockMenu.IsOpen)
        {
            Ui.BlockMenu.Close();
        }
    }

    private void LowerMarker(InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        if (isRotatingObject)
            return;

        CloseBlockSelection();

        marker.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y - GridSize, marker.transform.position.z);
        CreateGridFromMarker();
        lastMarkerMoveTime = Time.time;
    }

    private void RaiseMarker(InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        if (isRotatingObject)
            return;

        CloseBlockSelection();

        marker.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y + GridSize, marker.transform.position.z);
        CreateGridFromMarker();
        lastMarkerMoveTime = Time.time;
    }

    private void Remove(InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        if (isRotatingObject)
            ExitRotationMode();

        CloseBlockSelection();

        List<Block> blocks = CurrentWorld.GetBlocksAtPosition(MarkerPosition);
        Block block = blocks.Where(x => x.Info.Id == selectedBlock).FirstOrDefault();

        if (block != null)
        {
            CurrentWorld.Remove(block, MarkerPosition);
        }
        else
        {
            Debug.Log("No block of type " + BlockInfoLookup.Get(SelectedBlock).Name + " is placed here");
        }

        CurrentWorld.CalculateNeighbors();

        Builder.BuildWorld(CurrentWorld);
    }

    private void Place(InputAction.CallbackContext context)
    {
        if (controlsDisabled || isTestingLevel)
            return;

        if (isRotatingObject)
        {
            ExitRotationMode();
            return;
        }

        CloseBlockSelection();

        if (isPickingActivator)
        {
            List<Block> stateSwitchers = CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.Instance.GetComponent<StateSwitcher>() != null).ToList();
            PickedActivator(stateSwitchers.FirstOrDefault());
            return;
        }

        if (isPickingFinalPosition)
        {
            PickedMoveFinalPosition(MarkerPosition);
            return;
        }

        List<Block> sameBlocks = CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.BlockInfoId == selectedBlock).ToList();
        if (sameBlocks.Count() > 0)
        {
            Block selectedBlock = sameBlocks.First();
            Debug.Log("selected block: " + selectedBlock.ToString());
            Debug.Log(selectedBlock.HasPropertyExposer);
            controlsDisabled = true;
            WorldEditorUi.Instance.OpenBehaviourMenu(selectedBlock);
            return;
        }

        Block block = new Block(BlockInfoLookup.Get(SelectedBlock), MarkerPosition);
        CurrentWorld.Add(block);
        CurrentWorld.CalculateNeighbors();
        selectedWorldObject = block;

        Builder.BuildWorld(CurrentWorld);

        AudioManager.Instance.Play("place", ((block.Info.Width / 1.5f) / -4f) + 0.5f);

        if (selectedWorldObject.Info.RotatableX) //rotate objects that can be rotated on y axis to a random rotation when placing them
        {
            selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(0, 1, 0), Random.Range(0f, 360f));
            selectedWorldObject.Rotation = selectedWorldObject.Instance.transform.rotation;
            selectedWorldObject.RotationOffset = selectedWorldObject.Instance.transform.position - selectedWorldObject.Position;
            block.Rotation = selectedWorldObject.Rotation;
            block.RotationOffset = selectedWorldObject.RotationOffset;
        }
    }

    private void MoveMarkerCanceled(InputAction.CallbackContext context)
    {
        lastMarkerMoveTime = Time.time - MarkerMoveCooldown * 1.2f;
        currentLeftStickInput = Vector2.zero;
    }

    private void MoveMarker(InputAction.CallbackContext context)
    {
        if (marker == null)
        {
            Destroy(this);
            return;
        }

        if (controlsDisabled || isTestingLevel)
            return;

        Vector2 input = context.ReadValue<Vector2>();
        currentLeftStickInput = input;

        if (isRotatingObject)
            return;

        if (Time.time - lastMarkerMoveTime >= MarkerMoveCooldown)
        {
            int x = Mathf.Abs(input.x) > MarkerMoveSensitivity ? (input.x > 0 ? 1 : -1) : 0;
            int y = Mathf.Abs(input.y) > MarkerMoveSensitivity ? (input.y > 0 ? 1 : -1) : 0;
            Vector3 right = editorCamera.transform.right;

            if (Mathf.Abs(right.z) > Mathf.Abs(right.x))
            {
                int tempY = y;
                y = x;
                x = tempY;

                if (right.z < 0) // (0, 0, -1)
                {
                    y = y * -1;
                }
                else // (0, 0, 1)
                {
                    x = x * -1;
                }
            }
            else
            {
                if (right.x < 0) // (-1, 0, 0)
                {
                    x = x * -1;
                    y = y * -1;
                }
            }

            if (x != 0 || y != 0)
            {
                marker.transform.position = new Vector3(marker.transform.position.x + (x * GridSize), marker.transform.position.y, marker.transform.position.z + (y * GridSize));
                CreateGridFromMarker();
                lastMarkerMoveTime = Time.time;
                selectedWorldObject = CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.BlockInfoId == selectedBlock).FirstOrDefault();
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
