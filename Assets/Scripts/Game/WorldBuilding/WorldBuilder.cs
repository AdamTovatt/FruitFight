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
        
        foreach(BlockInfo blockInfo in blockInfoContainer.Infos)
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
        if(!prefabLookup.ContainsKey(name))
        {
            Debug.LogError("The prefab: " + name + " has not been loaded correctly. LoadPrefabs() should be called before getting a prefab");
            LoadPrefab(name);
        }

        return prefabLookup[name];
    }

    public void LoadPrefabs(World world)
    {
        foreach(Block block in world.Blocks)
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
        foreach(GameObject gameObject in previousWorldObjects)
        {
            Destroy(gameObject);
        }

        foreach(Block block in world.Blocks)
        {
            Instantiate(GetPrefab(block.Info.Prefab), new Vector3(block.X, block.Y, block.Z), Quaternion.identity, transform);
        }
    }
}
