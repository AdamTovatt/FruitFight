using Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public static WorldBuilder Instance;

    private List<GameObject> previousWorldObjects;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        previousWorldObjects = new List<GameObject>();
    }

    public void Build(string worldName)
    {
        World world = World.FromWorldName(worldName);
        BuildWorld(world);
    }

    public void BuildWorld(World world)
    {
        foreach (GameObject gameObject in previousWorldObjects)
        {
            Destroy(gameObject);
        }

        foreach (Block block in world.Blocks)
        {
            PlaceBlock(block);
        }
    }

    public void PlaceBlock(Block block)
    {
        if (block.Info.BlockType == BlockType.Large || block.Info.BlockType == BlockType.Invisible)
        {
            previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.Prefab), block.Position, Quaternion.identity, transform));
        }
        else if (block.Info.BlockType == BlockType.Ocean)
        {
            int width = block.Info.Width;
            for (int x = -5; x < 5; x++)
            {
                for (int y = -5; y < 5; y++)
                {
                    previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab("Water"), new Vector3(x * width, -2f, y * width), Quaternion.identity, transform));
                }
            }
        }

        if (block.Info.BlockType == BlockType.Large)
        {
            System.Random random = new System.Random(block.Position.GetSumOfComponents());

            if (block.NeighborX.Positive == null) //if we don't have a neighbor we should create an edge
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), block.Position, Quaternion.Euler(0, 90, 0), transform));
            if (block.NeighborX.Negative == null)
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), block.Position, Quaternion.Euler(0, -90, 0), transform));

            if (block.NeighborZ.Positive == null)
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), block.Position, Quaternion.Euler(0, 0, 0), transform));
            if (block.NeighborZ.Negative == null)
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), block.Position, Quaternion.Euler(0, 180, 0), transform));
        }
    }

    public void AddPreviousWorldObjects(GameObject gameObject)
    {
        previousWorldObjects.Add(gameObject);
    }
}
