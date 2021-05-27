using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class World
{
    public IReadOnlyList<Block> Blocks { get { return blocks; } }

    [SerializeField]
    private List<Block> blocks;

    private WorldBlockLookup blockLookup;

    public World()
    {
        blockLookup = new WorldBlockLookup();
        blocks = new List<Block>();
    }

    public void Add(Block block)
    {
        blocks.Add(block);
        blockLookup.Add(block);
    }

    public List<Block> GetBlocksAtPosition(Vector3Int position)
    {
        return blockLookup.GetBlocksAtPosition(position);
    }

    public static World FromJson(string json)
    {
        World world = JsonUtility.FromJson<World>(json);
        world.blockLookup.Initialize(world.blocks);
        world.CalculateNeighbors();
        return world;
    }

    public static World FromWorldName(string worldName)
    {
        return FromJson(WorldUtilities.LoadTextFile(string.Format("Maps/{0}", worldName)));
    }

    public void CalculateNeighbors()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].CalculateNeighbors(this);
        }
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }
}
