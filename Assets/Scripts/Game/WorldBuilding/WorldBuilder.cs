using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public static WorldBuilder Instance;
    public static Dictionary<int, BlockInfo> BlockInfoLookup;

    private Dictionary<string, GameObject> prefabLookup;
    private List<int> loadedBlocks;
    private BlockInfoContainer blockInfoContainer;

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
        blockInfoContainer = new BlockInfoContainer();
        loadedBlocks = new List<int>();
        prefabLookup = new Dictionary<string, GameObject>();
        BlockInfoLookup = new Dictionary<int, BlockInfo>();

        blockInfoContainer = JsonUtility.FromJson<BlockInfoContainer>(LoadTextFile("Configuration/BlockInfoContainer"));

        foreach (BlockInfo blockInfo in blockInfoContainer.Infos)
        {
            BlockInfoLookup.Add(blockInfo.Id, blockInfo);
        }
    }

    public static BlockInfo GetBlockInfo(int infoId)
    {
        if (!BlockInfoLookup.ContainsKey(infoId))
        {
            Debug.LogError("Missing key: " + infoId + " in BlockInfoLookup");
            return null;
        }

        return BlockInfoLookup[infoId];
    }

    public void Build(string worldName)
    {
        World world = World.FromJson(LoadTextFile(string.Format("Maps/{0}", worldName)));
        LoadPrefabs(world);
        BuildWorld(world);
    }

    public GameObject GetPrefab(string name)
    {
        if (!prefabLookup.ContainsKey(name))
        {
            Debug.LogError("The prefab: " + name + " has not been loaded correctly. LoadPrefabs() should be called before getting a prefab");
            LoadPrefab(name);
        }

        return prefabLookup[name];
    }

    public GameObject GetPrefab(List<string> nameVariations)
    {
        return GetPrefab(nameVariations[Random.Range(0, nameVariations.Count)]);
    }

    public void LoadPrefabs(World world)
    {
        foreach (Block block in world.Blocks)
        {
            if (!loadedBlocks.Contains(block.Info.Id))
            {
                List<string> prefabNames = new List<string>() { block.Info.Prefab };
                prefabNames.AddRange(block.Info.EdgePrefabs);

                foreach (string prefab in prefabNames)
                {
                    if (!prefabLookup.ContainsKey(prefab))
                    {
                        LoadPrefab(prefab);
                    }
                }

                loadedBlocks.Add(block.Info.Id);
            }
        }
    }

    private string LoadTextFile(string path)
    {
        return Resources.Load<TextAsset>(path).text;
    }

    private void LoadPrefab(string name)
    {
        prefabLookup.Add(name, Resources.Load<GameObject>(string.Format("Prefabs/Terrain/{0}", name)));
    }

    private void BuildWorld(World world)
    {
        foreach (GameObject gameObject in previousWorldObjects)
        {
            Destroy(gameObject);
        }

        foreach (Block block in world.Blocks)
        {
            if (block.Info.BlockType == BlockType.Large || block.Info.BlockType == BlockType.Invisible)
            {
                previousWorldObjects.Add(Instantiate(GetPrefab(block.Info.Prefab), block.Position, Quaternion.identity, transform));
            }
            else if (block.Info.BlockType == BlockType.Ocean)
            {
                int width = block.Info.Width;
                for (int x = -5; x < 5; x++)
                {
                    for (int y = -5; y < 5; y++)
                    {
                        previousWorldObjects.Add(Instantiate(GetPrefab("Water"), new Vector3(x * width, -2f, y * width), Quaternion.identity, transform));
                    }
                }
            }

            if (block.Info.BlockType == BlockType.Large)
            {
                if (block.NeighborX.Positive == null) //if we don't have a neighbor we should create an edge
                    previousWorldObjects.Add(Instantiate(GetPrefab(block.Info.EdgePrefabs), block.Position, Quaternion.Euler(0, 90, 0), transform));
                if (block.NeighborX.Negative == null)
                    previousWorldObjects.Add(Instantiate(GetPrefab(block.Info.EdgePrefabs), block.Position, Quaternion.Euler(0, -90, 0), transform));

                if (block.NeighborZ.Positive == null)
                    previousWorldObjects.Add(Instantiate(GetPrefab(block.Info.EdgePrefabs), block.Position, Quaternion.Euler(0, 0, 0), transform));
                if (block.NeighborZ.Negative == null)
                    previousWorldObjects.Add(Instantiate(GetPrefab(block.Info.EdgePrefabs), block.Position, Quaternion.Euler(0, 180, 0), transform));
            }
        }
    }

    public void AddPreviousWorldObjects(GameObject gameObject)
    {
        previousWorldObjects.Add(gameObject);
    }
}
