using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBlockLookup
{
    private Dictionary<int, Dictionary<int, Dictionary<int, List<Block>>>> blockLookup;

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
    }

    public List<Block> GetBlocksAtPosition(Vector3Int position)
    {
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
