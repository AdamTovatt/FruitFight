using Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public GameObject GridLinePrefab;
    public GameObject MainCameraPrefab;
    public GameObject MarkerPrefab;

    public float CameraSmoothTime = 1f;
    public float MarkerMoveCooldown = 0.5f;
    public float MarkerMoveSensitivity = 0.5f;

    public WorldBuilder Builder { get; private set; }
    public World CurrentWorld { get; private set; }

    public int GridSize { get; set; }
    public int SelectedBlock { get; set; }
    public Vector3Int MarkerPosition { get { return new Vector3Int(marker.position); } }

    private List<GameObject> gridLines;
    private MultipleTargetCamera mainCamera;
    private Transform marker;

    private float lastMarkerMoveTime;

    public void Awake()
    {
        GridSize = 4;
        SelectedBlock = 1;
        lastMarkerMoveTime = Time.time - MarkerMoveCooldown * 1.2f;

        gridLines = new List<GameObject>();
        CurrentWorld = new World();
        Builder = gameObject.GetComponent<WorldBuilder>();

        mainCamera = Instantiate(MainCameraPrefab).GetComponent<MultipleTargetCamera>();

        PlayerControls input = new PlayerControls();
        input.LevelEditor.Enable();
        input.LevelEditor.Place.performed += Place;
        input.LevelEditor.MoveMarker.performed += MoveMarker;
        input.LevelEditor.MoveMarker.canceled += (context) => { lastMarkerMoveTime = Time.time - MarkerMoveCooldown * 1.2f; };
    }

    private void Place(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Block block = new Block(BlockInfoLookup.Get(SelectedBlock), MarkerPosition);
        CurrentWorld.Add(block);
        CurrentWorld.CalculateNeighbors();

        Builder.BuildWorld(CurrentWorld);
    }

    private void MoveMarker(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (Time.time - lastMarkerMoveTime >= MarkerMoveCooldown)
        {
            Vector2 input = context.ReadValue<Vector2>();

            int x = Mathf.Abs(input.x) > MarkerMoveSensitivity ? (input.x > 0 ? 1 : -1) : 0;
            int y = Mathf.Abs(input.y) > MarkerMoveSensitivity ? (input.y > 0 ? 1 : -1) : 0;

            if (x != 0 || y != 0)
            {
                marker.position = new Vector3(marker.position.x + (x * GridSize), marker.position.y, marker.position.z + (y * GridSize));
                CreateGridFromMarker();
                lastMarkerMoveTime = Time.time;
            }
        }
    }

    void Start()
    {
        marker = Instantiate(MarkerPrefab).transform;
        CreateGridFromMarker();

        GetComponent<Spawner>().OnObjectSpawned += (sender, spawnedObject) =>
        {
            spawnedObject.GetComponentInChildren<SkyboxCamera>().SetMainCamera(mainCamera.transform);
        };

        mainCamera.Offset = mainCamera.Offset * 2;
        mainCamera.SmoothTime = CameraSmoothTime;
        mainCamera.Targets.Add(marker);
    }

    private Vector3 SetOnGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x / GridSize) * GridSize, Mathf.Round(position.y / GridSize) * GridSize, Mathf.Round(position.z / GridSize) * GridSize);
    }

    private void CreateGridFromMarker()
    {
        CreateGrid(4, new Vector3(marker.transform.position.x + GridSize / 2, marker.transform.position.y - GridSize, marker.transform.position.z + GridSize / 2));
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
