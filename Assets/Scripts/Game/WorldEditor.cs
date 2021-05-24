using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public GameObject GridLinePrefab;
    public GameObject MainCameraPrefab;

    public WorldBuilder Builder { get; private set; }
    public World World { get; private set; }

    private List<GameObject> gridLines;
    private MultipleTargetCamera mainCamera;

    public void Awake()
    {
        gridLines = new List<GameObject>();
        World = new World();

        mainCamera = Instantiate(MainCameraPrefab).GetComponent<MultipleTargetCamera>();
    }

    void Start()
    {
        CreateGrid(2, 4, new Vector3(0, 0, 0));
        FindObjectOfType<SkyboxCamera>().SetMainCamera(mainCamera.transform);
    }

    private void CreateGrid(int tileSize, int tiles, Vector3 centerPoint)
    {
        for (int x = -tiles; x < tiles + 1; x++)
        {
            for (int z = -tiles; z < tiles + 1; z++)
            {
                Vector3 currentPosition = new Vector3(centerPoint.x + x * tileSize, centerPoint.y, centerPoint.z + z * tileSize);
                Vector3 nextPositionX = new Vector3(centerPoint.x + (x + 1) * tileSize, centerPoint.y, centerPoint.z + z * tileSize);
                Vector3 nextPositionZ = new Vector3(centerPoint.x + x * tileSize, centerPoint.y, centerPoint.z + (z + 1) * tileSize);

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
