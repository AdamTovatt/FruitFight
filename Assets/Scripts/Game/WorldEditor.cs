using Assets.Scripts.Models;
using Lookups;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public float TimeToMaxMarkerSpeed = 2f;
    public float MarkerMaxSpeed = 0.8f;
    public float MarkerMoveSensitivity = 0.5f;
    public float ObjectRotationSpeed = 180f;

    public delegate void ImageWasCaptured(Texture2D image);
    public event ImageWasCaptured OnImageWasCaptured;

    public delegate void ActivatorWasPicked(Block block);
    public event ActivatorWasPicked OnActivatorWasPicked;

    public delegate void PositionWasPicked(Vector3Int position);
    public event PositionWasPicked OnPositionWasPicked;

    public delegate void TriggerZoneWasPicked(Block block);
    public event TriggerZoneWasPicked OnTriggerZoneWasPicked;

    public WorldBuilder Builder { get; private set; }
    public World CurrentWorld { get; private set; }
    public bool ControlsDisabled { get; set; }

    public BlockThumbnailManager ThumbnailManager { get; private set; }

    public int GridSize { get; set; }
    public int SelectedBlock { get { return selectedBlock; } set { SetSelectedBlock(value); } } //this is the selected block in the blockselection ui menu. Is the block info id
    private int selectedBlock;

    public Vector3Int MarkerPosition { get { return new Vector3Int(marker.transform.position); } }
    public static bool IsTestingLevel { get { return isTestingLevel; } }

    private List<GameObject> gridLines;
    private EditorCamera editorCamera;
    private EditorMarker marker;
    private static bool isTestingLevel = false;
    private bool isRotatingObject = false;

    private bool isPickingActivator = false;
    private bool isPickingActivator2 = false;
    private List<Block> currentlyAvailableActivators;
    private int currentActivatorIndex;
    private Block currentMovePropertiesBlock;
    private Block currentTriggerZonePropertiesBlock;

    private bool isCapturingImage = false;
    private bool isAddingTriggerSubZone = false;
    private bool isPickingFinalPosition = false;
    private bool isPickingFinalPosition2 = false;
    private Vector2 currentLeftStickInput;
    private float moveMarkerStartTime = 0;
    private Block selectedWorldObject; //this is the selected object in the world, the object which the marker is above
    private MonoBehaviour activatorPickingMenu;

    private float lastMarkerMoveTime;
    private float lastPageSwitchTime;
    private float zoomSpeedMultiplier = 1;
    private float lastRotateTime;
    private bool hasReleasedRotate;

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

        AddInputEventListeners();
    }

    void Start()
    {
        marker = Instantiate(MarkerPrefab).GetComponent<EditorMarker>();
        CreateGridFromMarker();

        GetComponent<Spawner>().OnObjectSpawned += SpawnerSpawnedObject;

        editorCamera.Target = marker.transform;

        Ui = FindObjectOfType<WorldEditorUi>();

        if (DaylightController.Instance != null)
            DaylightController.Instance.Initialize(CurrentWorld.NorthRotation, CurrentWorld.TimeOfDay);
    }

    private void Update()
    {
        if (isRotatingObject)
        {
            float x = selectedWorldObject.Info.RotatableX ? currentLeftStickInput.x : 0;
            float y = selectedWorldObject.Info.RotatableY ? currentLeftStickInput.y : 0;

            if (selectedWorldObject.Info.AngleLimit != 0)
            {
                if (hasReleasedRotate || Time.time - lastRotateTime > 0.4f)
                {
                    if (x != 0)
                        selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(0, x, 0), selectedWorldObject.Info.AngleLimit);
                    if (y != 0)
                        selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(y, 0, 0), selectedWorldObject.Info.AngleLimit);

                    lastRotateTime = Time.time;
                    hasReleasedRotate = false;
                }
            }
            else
            {
                selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(0, x, 0), Time.deltaTime * ObjectRotationSpeed * Mathf.Abs(x));
                selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(y, 0, 0), Time.deltaTime * ObjectRotationSpeed * Mathf.Abs(y));
            }

            if (x == 0 && y == 0 && !hasReleasedRotate)
                hasReleasedRotate = true;
        }
        else
        {
            if (currentLeftStickInput != Vector2.zero)
                MoveMarker();
        }
    }

    public void ExitButtonWasPressed()
    {
        RemoveInputEventListeners();
        WorldEditorUi.Instance.Destroy();
        SceneManager.LoadScene("MainMenuScene");
    }

    private void SpawnerSpawnedObject(object sender, GameObject spawnedObject)
    {
        spawnedObject.GetComponentInChildren<SkyboxCamera>().SetMainCamera(editorCamera.transform);
        GetComponent<Spawner>().OnObjectSpawned -= SpawnerSpawnedObject;
    }

    public void PickPosition2(Block block)
    {
        WorldEditorUi.Instance.DisableUiInput();
        EnableControls();
        isPickingFinalPosition2 = true;
        SetSelectedBlock(block.BlockInfoId);
        SetMarkerPositionToBlock(block);
        currentMovePropertiesBlock = block;
    }

    public void PickedPosition2(Vector3Int position)
    {
        WorldEditorUi.Instance.EnableUiInput();
        ControlsDisabled = true;
        isPickingFinalPosition2 = false;

        SetMarkerPositionToBlock(currentMovePropertiesBlock);

        OnPositionWasPicked?.Invoke(position);
        OnPositionWasPicked = null;
    }

    public void AddTriggerSubZone(Block block)
    {
        WorldEditorUi.Instance.DisableUiInput();
        EnableControls();
        SetSelectedBlock(block.BlockInfoId);
        SetMarkerPositionToBlock(block);
        isAddingTriggerSubZone = true;
        currentTriggerZonePropertiesBlock = block;
    }

    private void AddedTriggerSubZone(Block triggerSubZone)
    {
        WorldEditorUi.Instance.EnableUiInput();
        SetMarkerPositionToBlock(currentTriggerZonePropertiesBlock);
        isAddingTriggerSubZone = false;
        ControlsDisabled = true;
        OnTriggerZoneWasPicked?.Invoke(triggerSubZone);
        OnTriggerZoneWasPicked = null;
    }

    public void CaptureImage()
    {
        Ui.LevelPropertiesScreen.gameObject.SetActive(false);
        EnableControls();
        Ui.DisableUiInput();
        marker.Hide();
        isCapturingImage = true;
    }

    public void CaptureImageWasCompleted(Texture2D image)
    {
        OnImageWasCaptured?.Invoke(image);
        OnImageWasCaptured = null;
        Ui.LevelPropertiesScreen.gameObject.SetActive(true);
        marker.Show();
        DisableControls();
        Ui.EnableUiInput();
        isCapturingImage = false;
    }

    public List<Block> GetCurrentlyAvailableActivators()
    {
        currentlyAvailableActivators = new List<Block>();
        foreach (StateSwitcher stateSwitcher in FindObjectsOfType<StateSwitcher>())
            currentlyAvailableActivators.Add(CurrentWorld.Blocks.Where(x => x.Instance == stateSwitcher.gameObject).FirstOrDefault());

        currentlyAvailableActivators = currentlyAvailableActivators.OrderBy(x => ((Vector3)(x.Position - MarkerPosition)).sqrMagnitude).ToList();

        return currentlyAvailableActivators;
    }

    public void PickActivator2(Block blockOfOrigin)
    {
        GetCurrentlyAvailableActivators();

        currentActivatorIndex = -1;
        WorldEditorUi.Instance.DisableUiInput();
        EnableControls();
        isPickingActivator2 = true;
        currentMovePropertiesBlock = blockOfOrigin;
    }

    private void PickedActivator2(Block selectedStateSwitcher)
    {
        WorldEditorUi.Instance.EnableUiInput();
        DisableControls();
        isPickingActivator2 = false;

        OnActivatorWasPicked?.Invoke(selectedStateSwitcher);
        OnActivatorWasPicked = null;

        currentlyAvailableActivators = null;

        SetMarkerPositionToBlock(currentMovePropertiesBlock);
    }

    private bool triedSavingWithoutSuccess = false;
    private bool showedWarning = false;

    public void SaveLevel()
    {
        if (CurrentWorld.Metadata == null || string.IsNullOrEmpty(CurrentWorld.Metadata.Name)) //the player needs to go to level properties before saving
        {
            if (!triedSavingWithoutSuccess)
            {
                AlertCreator.Instance.CreateNotification("You need to chose a name for this level first! Please go to Level Properties");
                triedSavingWithoutSuccess = true;
            }
            else
            {
                if (!showedWarning)
                {
                    AlertCreator.Instance.CreateNotification("If you click Save again you will be taken to Level Properties");
                    showedWarning = true;
                }
                else
                {
                    Ui.ShowLevelProperties();
                    triedSavingWithoutSuccess = false;
                    showedWarning = false;
                }
            }

            return;
        }

        FileHelper.SaveWorld(CurrentWorld);

        AlertCreator.Instance.CreateNotification("Level was saved!", 2f);
    }

    private void LoadLevelFromMetadata(WorldMetadata metadata)
    {
        string mapDirectory = string.Format("{0}/maps", Application.persistentDataPath);

        if (!Directory.Exists(mapDirectory))
            Directory.CreateDirectory(mapDirectory);

        string levelPath = string.Format("{0}/{1}.mapdata", mapDirectory, metadata.Name.Replace(' ', '_'));

        if (File.Exists(levelPath))
        {
            CurrentWorld = World.FromJson(File.ReadAllText(levelPath));
            Builder.BuildWorld(CurrentWorld);
        }
        else
        {
            Ui.AlertCreator.CreateNotification(string.Format("No such level found: {0}", levelPath), 4f);
        }
    }

    private void OnLevelWasSelected(WorldMetadata metadata)
    {
        LoadLevelFromMetadata(metadata);

        WorldEditorUi.Instance.LoadLevelScreen.Close();
        Ui.ClosePauseMenu();

        Ui.AlertCreator.CreateNotification(string.Format("Loaded the level: {0}", metadata.Name), 3f);
    }

    public void LoadLevel()
    {
        WorldEditorUi.Instance.LoadLevelScreen.gameObject.SetActive(true);
        WorldEditorUi.Instance.LoadLevelScreen.Show().OnLevelWasSelected += OnLevelWasSelected;
    }

    private void Grassify(InputAction.CallbackContext context)
    {
        if (ControlsDisabled)
            return;

        GrassifyConfiguration grassifyConfiguration = GrassifyConfiguration.LoadFromConfig();

        List<Vector3Int> placedPositions = new List<Vector3Int>();
        List<Block> blocksToAdd = new List<Block>();

        foreach (Block growBlock in CurrentWorld.Blocks) //check all blocks in world
        {
            if (grassifyConfiguration.GrowBlocks.Contains(growBlock.Info.Id)) //if this block is a block that should grow from
            {
                if (growBlock.NeighborY.SameTypesPositive.Count == 0) //make sure there are no blocks of the same type above this block
                {
                    foreach (GrassifyBlockConfiguration blockConfiguration in grassifyConfiguration.VegetationBlocks) //go through all the vegetationblocks
                    {
                        BlockInfo grassInfo = BlockInfoLookup.Get(blockConfiguration.Id);

                        //limit obscured positions to only the ones that are on the same y level as the current block and are spaced with the same distance as the vegetation between
                        IEnumerable<Vector3Int> filteredPositions = growBlock.ObscuredPositions.Where(p => p.Y == growBlock.Y && !placedPositions.Contains(p));

                        if (growBlock.Info.Width > grassInfo.Width)
                            filteredPositions = filteredPositions.Where(p => p.X % grassInfo.Width == 0 && p.Z % grassInfo.Width == 0);
                        else
                            filteredPositions = filteredPositions.Take(1);

                        foreach (Vector3Int obscuredPosition in filteredPositions.ToList())
                        {
                            if (Random.Range(0f, 1f) < blockConfiguration.Probability)
                            {
                                int blockId = blockConfiguration.Id;

                                if (Random.Range(0f, 1f) < blockConfiguration.VariationProbability) //take random variation id
                                {
                                    if (blockConfiguration.Variations.Count > 0)
                                    {
                                        blockId = blockConfiguration.Variations[Random.Range(0, blockConfiguration.Variations.Count)];
                                    }
                                }

                                Block newBlock = new Block(BlockInfoLookup.Get(blockId), obscuredPosition) { IsFromGrassify = true };
                                newBlock.BehaviourProperties2.Add(new ChildObject.ChildObjectProperties(growBlock.Id)); //make this vegetation have the growblock as a parent

                                blocksToAdd.Add(newBlock); //add block that is sometimes of a random variation

                                if (!blockConfiguration.AllowOverlap)
                                    placedPositions.Add(obscuredPosition); //so that we won't place multiple things on same place
                            }
                        }
                    }
                }
            }
        }

        foreach (Block block in CurrentWorld.Blocks.Where(x => x.IsFromGrassify))
        {
            CurrentWorld.Remove(block, block.Position);
        }

        foreach (Block block in blocksToAdd)
        {
            CurrentWorld.Add(block);
        }

        CurrentWorld.CalculateNeighbors();
        Builder.BuildWorld(CurrentWorld);

        foreach (Block block in blocksToAdd)
        {
            if (block.Info.RotatableX) //rotate objects that can be rotated on y axis to a random rotation when placing them
            {
                block.Instance.transform.RotateAround(block.CenterPoint, new Vector3(0, 1, 0), Random.Range(0f, 360f));
                block.Rotation = block.Instance.transform.rotation;
                block.RotationOffset = block.Instance.transform.position - block.Position;
            }
        }

        Ui.AlertCreator.CreateNotification("Vegetation was automatically added", 2);
    }

    public void TestLevelButton()
    {
        if (CurrentWorld.Blocks.Where(x => x.BlockInfoId == 2).Count() < 1)
        {
            WorldEditorUi.Instance.AlertCreator.CreateNotification("No player startpoint exists in level. Add one first");
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
            marker.transform.position = SetOnGrid(MarkerPosition);
            CreateGridFromMarker();
            marker.GetComponent<EditorMarker>().SetMarkerSize(new Vector2Int(block.Width, block.Width));
        }
    }

    private void StartLevelTest()
    {
        GameStateManager.SetGameState(GameState.Free);
        Ui.EventSystem.SetActive(false);
        Ui.ShowLoadingScreen();
        Ui.ClosePauseMenu();
        Ui.HideBlockSelection();
        WorldBuilder.NextLevel = CurrentWorld;
        SceneManager.LoadScene("PlayerSetup");
        SceneManager.sceneLoaded += LevelTestWasLoaded;
        isTestingLevel = true;
        ControlsDisabled = false;
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
        RemoveInputEventListeners();

        WorldBuilder.Instance.BuildWorld(CurrentWorld);
        Instance.CurrentWorld = CurrentWorld;

        if (Ui.EventSystem != null)
            Ui.EventSystem.SetActive(true);
    }

    private void AddInputEventListeners()
    {
        input.LevelEditor.Place.performed += Place;
        input.LevelEditor.MoveMarker.performed += MoveMarkerPerformed;
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
        input.LevelEditor.RightClick.performed += OpenBehaviourMenu;
        input.LevelEditor.Rotate.performed += editorCamera.Rotate;
        input.LevelEditor.Rotate.canceled += editorCamera.CancelRotate;
        input.LevelEditor.ToggleObjectRotation.performed += ToggleRotateObject;
        input.LevelEditor.MouseRotate.performed += editorCamera.MouseRotate;
        input.LevelEditor.MouseRotate.canceled += editorCamera.CancelMouseRotate;
        input.LevelEditor.MouseWheel.performed += editorCamera.ScrollWheelDown;
        input.LevelEditor.MouseWheel.canceled += editorCamera.ScrollWheelUp;
        input.LevelEditor.Grassify.performed += Grassify;
        input.LevelEditor.MouseScroll.performed += MouseScroll;
    }

    public void RemoveInputEventListeners()
    {
        SceneManager.sceneLoaded -= LevelEditorWasLoaded;
        input.LevelEditor.Place.performed -= Place;
        input.LevelEditor.MoveMarker.performed -= MoveMarkerPerformed;
        input.LevelEditor.RaiseMarker.performed -= RaiseMarker;
        input.LevelEditor.LowerMarker.performed -= LowerMarker;
        input.LevelEditor.MoveMarker.canceled -= MoveMarkerCanceled;
        input.LevelEditor.Pause.performed -= Pause;
        input.LevelEditor.MoveBlockSelection.performed -= MoveBlockSelection;
        input.LevelEditor.NextPage.performed -= NextPage;
        input.LevelEditor.PreviousPage.performed -= PreviousPage;
        input.LevelEditor.NextPage.canceled -= CancelNextPage;
        input.LevelEditor.PreviousPage.canceled -= CancelPreviousPage;
        input.LevelEditor.RightClick.performed -= OpenBehaviourMenu;
        input.LevelEditor.Rotate.performed -= editorCamera.Rotate;
        input.LevelEditor.Rotate.canceled -= editorCamera.CancelRotate;
        input.LevelEditor.MouseRotate.performed -= editorCamera.MouseRotate;
        input.LevelEditor.MouseRotate.canceled -= editorCamera.CancelMouseRotate;
        input.LevelEditor.MouseWheel.performed -= editorCamera.ScrollWheelDown;
        input.LevelEditor.MouseWheel.canceled -= editorCamera.ScrollWheelUp;
        input.LevelEditor.ToggleObjectRotation.performed -= ToggleRotateObject;
        input.LevelEditor.Grassify.performed -= Grassify;
        input.LevelEditor.MouseScroll.performed -= MouseScroll;
    }

    public void ExitLevelTest()
    {
        Ui.ShowLoadingScreen();
        isTestingLevel = false;
        ControlsDisabled = true;

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
            if (isPickingFinalPosition2)
                PickedPosition2(null);
            else if (isPickingActivator2)
                PickedActivator2(null);
            else if (isCapturingImage)
                CaptureImageWasCompleted(null);
            else if (isRotatingObject)
                ExitRotationMode();
            else
                Ui.EscapeWasPressed();
        }
        else
        {
            if (!ControlsDisabled)
            {
                ControlsDisabled = true;
            }
            else
            {
                ControlsDisabled = false;
            }
        }
    }

    public void EnableControls()
    {
        Debug.Log("Enable controls");
        ControlsDisabled = false;
    }

    public void DisableControls()
    {
        Debug.Log("Disable controls");
        ControlsDisabled = true;
    }

    private void CancelNextPage(InputAction.CallbackContext context)
    {
        editorCamera.EndZoomIn();
    }

    private void CancelPreviousPage(InputAction.CallbackContext context)
    {
        editorCamera.EndZoomOut();
    }

    private void MouseScroll(InputAction.CallbackContext context)
    {
        if (ControlsDisabled)
            return;

        float scrollValue = context.ReadValue<float>();

        if (scrollValue > 0)
        {
            zoomSpeedMultiplier = 10;
            NextPage(context);
        }
        else if (scrollValue < 0)
        {
            zoomSpeedMultiplier = 10;
            PreviousPage(context);
        }
        else
        {
            zoomSpeedMultiplier = 1;
            editorCamera.EndZoomIn();
            editorCamera.EndZoomOut();
        }
    }

    private void NextPage(InputAction.CallbackContext context)
    {
        if (!ControlsDisabled || isTestingLevel)
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
                editorCamera.StartZoomIn(zoomSpeedMultiplier);
            }
        }
    }

    private void PreviousPage(InputAction.CallbackContext context)
    {
        if (!ControlsDisabled || isTestingLevel)
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
                editorCamera.StartZoomOut(zoomSpeedMultiplier);
            }
        }
    }

    private void ToggleRotateObject(InputAction.CallbackContext context)
    {
        if (ControlsDisabled)
            return;

        if (!isRotatingObject)
        {
            if (selectedWorldObject != null)
            {
                if (selectedWorldObject.Info.RotatableX || selectedWorldObject.Info.RotatableY)
                {
                    isRotatingObject = true;
                    AlertCreator.Instance.CreateNotification("to rotate object", 2f, "Move");
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
        AlertCreator.Instance.CreateNotification("Rotate mode exited", 2f);
    }

    private void MoveBlockSelection(InputAction.CallbackContext context)
    {
        if (!isPickingFinalPosition && !isPickingFinalPosition2)
        {
            if (!ControlsDisabled || isTestingLevel)
            {
                if (isRotatingObject)
                    ExitRotationMode();

                Vector2 rawMoveValue = context.ReadValue<Vector2>();

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
        if (ControlsDisabled || isTestingLevel)
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
        if (ControlsDisabled || isTestingLevel)
            return;

        if (isRotatingObject)
            return;

        CloseBlockSelection();

        marker.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y + GridSize, marker.transform.position.z);
        CreateGridFromMarker();
        lastMarkerMoveTime = Time.time;
    }

    private void RemoveBlockAtPosition(Vector3Int position)
    {
        if (isRotatingObject)
            ExitRotationMode();

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

    private void OpenBehaviourMenu(InputAction.CallbackContext context)
    {
        if (ControlsDisabled || IsTestingLevel)
            return;

        WorldEditorUi.Instance.OpenBehaviourMenu();
    }

    public Block GetCurrentHighlightedBlock()
    {
        return CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.BlockInfoId == selectedBlock).ToList().FirstOrDefault();
    }

    private IEnumerator CaptureScreenShot()
    {
        yield return new WaitForEndOfFrame();

        Texture2D image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        image.Apply();

        CaptureImageWasCompleted(image);
    }

    private void Place(InputAction.CallbackContext context)
    {
        if (ControlsDisabled || isTestingLevel || Ui.BlockMenu.IsOpen || Ui.BlockMenu.WasJustClosed)
            return;

        if (isCapturingImage)
        {
            StartCoroutine(CaptureScreenShot());
            return;
        }

        if (Ui.PauseMenuIsOpen)
            return;

        if (isRotatingObject)
        {
            ExitRotationMode();
            return;
        }

        if (isPickingActivator2)
        {
            List<Block> stateSwitchers = CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.Instance.GetComponent<StateSwitcher>() != null).ToList();
            PickedActivator2(stateSwitchers.FirstOrDefault());
            return;
        }

        if (isPickingFinalPosition2)
        {
            PickedPosition2(MarkerPosition);
            return;
        }

        List<Block> sameBlocks = CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.BlockInfoId == selectedBlock).ToList();
        if (sameBlocks.Count() > 0)
        {
            RemoveBlockAtPosition(MarkerPosition);
            return;
        }

        Block block = new Block(BlockInfoLookup.Get(SelectedBlock), MarkerPosition);

        if (isAddingTriggerSubZone) //we should set this trigger sub zone to not be a parent
        {
            TriggerZone.TriggerZoneProperties properties = new TriggerZone.TriggerZoneProperties(currentTriggerZonePropertiesBlock.Id);
            block.BehaviourProperties2.Add(properties);
        }

        CurrentWorld.Add(block);
        CurrentWorld.CalculateNeighbors();
        selectedWorldObject = block;

        Builder.BuildWorld(CurrentWorld);

        AudioManager.Instance.Play("place", ((block.Info.Width / 1.5f) / -4f) + 0.5f);

        if (selectedWorldObject.Info.RotatableX && selectedWorldObject.Info.AngleLimit == 0) //rotate objects that can be rotated on y axis to a random rotation when placing them (only if no angle limit exists)
        {
            selectedWorldObject.Instance.transform.RotateAround(selectedWorldObject.CenterPoint, new Vector3(0, 1, 0), Random.Range(0f, 360f));
            selectedWorldObject.Rotation = selectedWorldObject.Instance.transform.rotation;
            selectedWorldObject.RotationOffset = selectedWorldObject.Instance.transform.position - selectedWorldObject.Position;
            block.Rotation = selectedWorldObject.Rotation;
            block.RotationOffset = selectedWorldObject.RotationOffset;
        }

        if (isAddingTriggerSubZone)
            AddedTriggerSubZone(block); //if we were adding a trigger zone we should do a callback to the trigger zone menu now
    }

    public void RebuildCurrentWorld()
    {
        Builder.BuildWorld(CurrentWorld);
    }

    private void MoveMarker()
    {
        if (isRotatingObject)
            return;

        float appliedMarkerCooldown = MarkerMoveCooldown - Mathf.Clamp((Time.time - moveMarkerStartTime) / TimeToMaxMarkerSpeed, 0, MarkerMoveCooldown * MarkerMaxSpeed);
        if (Time.time - lastMarkerMoveTime >= appliedMarkerCooldown)
        {
            int x = Mathf.Abs(currentLeftStickInput.x) > MarkerMoveSensitivity ? (currentLeftStickInput.x > 0 ? 1 : -1) : 0;
            int y = Mathf.Abs(currentLeftStickInput.y) > MarkerMoveSensitivity ? (currentLeftStickInput.y > 0 ? 1 : -1) : 0;
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
                if (!isPickingActivator && !isPickingActivator2)
                {
                    marker.transform.position = new Vector3(marker.transform.position.x + (x * GridSize), marker.transform.position.y, marker.transform.position.z + (y * GridSize));
                }
                else
                {
                    currentActivatorIndex += x + y;

                    if (currentActivatorIndex < 0)
                        currentActivatorIndex = currentlyAvailableActivators.Count - 1;
                    if (currentActivatorIndex >= currentlyAvailableActivators.Count)
                        currentActivatorIndex = 0;

                    Block currentActivator = currentlyAvailableActivators[currentActivatorIndex];
                    SetMarkerPositionToBlock(currentActivator);
                }

                CreateGridFromMarker();
                lastMarkerMoveTime = Time.time;
                selectedWorldObject = CurrentWorld.GetBlocksAtPosition(MarkerPosition).Where(b => b.BlockInfoId == selectedBlock).FirstOrDefault();
            }

            CloseBlockSelection();
        }
    }

    private void MoveMarkerCanceled(InputAction.CallbackContext context)
    {
        lastMarkerMoveTime = Time.time - MarkerMoveCooldown * 1.2f;
        currentLeftStickInput = Vector2.zero;
    }

    private void MoveMarkerPerformed(InputAction.CallbackContext context)
    {
        if (marker == null)
        {
            Destroy(this);
            return;
        }

        if (ControlsDisabled || isTestingLevel)
            return;

        Vector2 input = context.ReadValue<Vector2>();
        currentLeftStickInput = input;
        moveMarkerStartTime = Time.time;
    }

    private void SetMarkerPositionToBlock(Block block)
    {
        marker.transform.position = block.Position;
        GridSize = block.Info.Width;
        marker.GetComponent<EditorMarker>().SetMarkerSize(new Vector2Int(block.Info.Width, block.Info.Width));
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

        if (isCapturingImage)
            return;

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
