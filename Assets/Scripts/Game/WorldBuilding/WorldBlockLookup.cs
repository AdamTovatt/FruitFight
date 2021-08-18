using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void Remove(Block block, Vector3Int position)
    {
        for (int x = 0; x < block.Info.Width; x++)
        {
            for (int z = 0; z < block.Info.Width; z++)
            {
                for (int y = 0; y < block.Info.Height; y++)
                {
                    List<Block> blocksAtPosition = blockLookup[position.Z + z][position.X + x][position.Y - y];
                    blockLookup[position.Z + z][position.X + x][position.Y - y] = blocksAtPosition.Where(x => x.Id != block.Id).ToList();
                }
            }
        }
    }

    public void Add(Block block)
    {
        block.ObscuredPositions = new List<Vector3Int>();

        for (int x = 0; x < block.Info.Width; x++)
        {
            for (int z = 0; z < block.Info.Width; z++)
            {
                for (int y = 0; y < block.Info.Height; y++)
                {
                    Vector3Int position = new Vector3Int(block.X + x, block.Y - y, block.Z + z);
                    Add(block, position);
                    block.ObscuredPositions.Add(position);
                }
            }
        }
    }

    private void Add(Block block, Vector3Int position)
    {
        if (!blockLookup.ContainsKey(position.Z))
            blockLookup.Add(position.Z, new Dictionary<int, Dictionary<int, List<Block>>>());

        Dictionary<int, Dictionary<int, List<Block>>> zLevel = blockLookup[position.Z];

        if (!zLevel.ContainsKey(position.X))
            zLevel.Add(position.X, new Dictionary<int, List<Block>>());

        Dictionary<int, List<Block>> xLevel = zLevel[position.X];

        if (!xLevel.ContainsKey(position.Y))
            xLevel.Add(position.Y, new List<Block>());

        List<Block> blocksAtPosition = xLevel[position.Y];

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
