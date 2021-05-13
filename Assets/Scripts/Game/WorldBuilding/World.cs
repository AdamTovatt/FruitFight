using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class World
{
    public List<Block> Blocks;

    public World()
    {
        Blocks = new List<Block>();
    }

    public void Add(Block block)
    {
        Blocks.Add(block);
    }

    public static World FromJson(string json)
    {
        World world = JsonUtility.FromJson<World>(json);
        world.CalculateNeighbors();
        return world;
    }

    private void CalculateNeighbors()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Block block = Blocks[i];
        }
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }
}
