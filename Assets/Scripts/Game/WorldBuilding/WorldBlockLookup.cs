using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBlockLookup
{
    private Dictionary<int, Dictionary<int, Dictionary<int, List<Block>>>> blockLookup;
    private bool initialized = false;

    public WorldBlockLookup()
    {
        blockLookup = new Dictionary<int, Dictionary<int, Dictionary<int, List<Block>>>>();
    }

    public void Initialize(List<Block> blocks)
    {
        foreach(Block block in blocks)
        {
            Add(block);
        }

        initialized = true;
    }

    public void Add(Block block)
    {
        if (!blockLookup.ContainsKey(block.Z))
            blockLookup.Add(block.Z, new Dictionary<int, Dictionary<int, List<Block>>>());

        Dictionary<int, Dictionary<int, List<Block>>> zLevel = blockLookup[block.Z];

        if (!zLevel.ContainsKey(block.X))
            zLevel.Add(block.X, new Dictionary<int, List<Block>>());

        Dictionary<int, List<Block>> xLevel = zLevel[block.X];

        if (!xLevel.ContainsKey(block.Y))
            xLevel.Add(block.Y, new List<Block>());

        List<Block> blocksAtPosition = xLevel[block.Y];

        blocksAtPosition.Add(block);

        if (!initialized)
            initialized = true;
    }

    public List<Block> GetBlocksAtPosition(Vector3Int position)
    {
        if(!initialized)
        {
            Debug.LogError("A WorldBlockLookup that hasn't been initialized was used. It has to be initialized first");
            return null;
        }

        if (!blockLookup.ContainsKey(position.Z))
            return new List<Block>();

        Dictionary<int, Dictionary<int, List<Block>>> zLevel = blockLookup[position.Z];

        if (!zLevel.ContainsKey(position.X))
            return new List<Block>();

        Dictionary<int, List<Block>> xLevel = zLevel[position.X];

        if (!xLevel.ContainsKey(position.Y))
            return new List<Block>();

        return xLevel[position.Y];
    }
}
