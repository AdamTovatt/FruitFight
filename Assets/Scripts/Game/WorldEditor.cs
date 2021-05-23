using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public GameObject GridLinePrefab;

    public WorldBuilder Builder { get; private set; }
    public World World { get; private set; }

    private List<GameObject> gridLines;

    public void Awake()
    {
        gridLines = new List<GameObject>();
        World = new World();
    }

    void Start()
    {
        CreateGrid(2, 4, new Vector3(0, 0, 0));
    }

    private void CreateGrid(int tileSize, int tiles, Vector3 centerPoint)
    {
        for (int x = 0; x < tiles * 2; x++)
        {
            for (int z = 0; z < tiles * 2; z++)
            {
                int appliedX = tiles * 2 - x;
                int appliedZ = tiles * 2 - z;

                Vector3 currentPosition = new Vector3(centerPoint.x + appliedX * tileSize, centerPoint.y, centerPoint.z + appliedZ * tileSize);
                Vector3 nextPositionX = new Vector3(centerPoint.x + (appliedX + 1) * tileSize, centerPoint.y, centerPoint.z + appliedZ * tileSize);
                Vector3 nextPositionZ = new Vector3(centerPoint.x + appliedX * tileSize, centerPoint.y, centerPoint.z + (appliedZ + 1) * tileSize);

                if (appliedX + 1 <= tiles * tileSize)
                {
                    GameObject gridLineX = Instantiate(GridLinePrefab, transform);
                    gridLines.Add(gridLineX);

                    LineRenderer lineX = gridLineX.GetComponent<LineRenderer>();
                    lineX.SetPosition(0, currentPosition);
                    lineX.SetPosition(1, nextPositionX);
                }

                if (appliedZ <= tileSize * tileSize)
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
