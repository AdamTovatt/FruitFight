using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockInfo
{
    public int Id;
    public BlockType BlockType;
    public int Width;
    public int Height;
    public bool ShowInEditor;
    public string Name;
    public string Prefab;
    public List<string> EdgePrefabs;
    public bool RotatableX;
    public bool RotatableY;
    public float AngleLimit;
    public bool StartWithPropertyExposer;
    public BlockMaterial BlockMaterial;
    public List<string> AvailableBehaviours;
    public List<BehaviourProperties> DefaultBehaviours;

    public BlockInfo()
    {
        EdgePrefabs = new List<string>();
    }
}

public enum BlockType
{
    Block, Invisible, Ocean, Prop, Detail
}

public enum BlockMaterial
{
    NotSpecified, Grass, Wood, Stone
}

[Serializable]
public class BlockInfoContainer
{
    public List<BlockInfo> Infos;

    public BlockInfoContainer()
    {
        Infos = new List<BlockInfo>();
    }

    public static BlockInfoContainer LoadFromConfiguration()
    {
        return JsonConvert.DeserializeObject<BlockInfoContainer>(WorldUtilities.LoadTextFile("Configuration/BlockInfoContainer"));
    }

    public Dictionary<int, BlockInfo> CreateLookup()
    {
        Dictionary<int, BlockInfo> result = new Dictionary<int, BlockInfo>();

        foreach (BlockInfo blockInfo in Infos)
        {
            result.Add(blockInfo.Id, blockInfo);
        }

        return result;
    }
}