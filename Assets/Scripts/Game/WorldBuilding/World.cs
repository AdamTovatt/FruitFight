using System;
using System.Collections.Generic;
using System.Linq;
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

    public static World FromWorldName(string worldName)
    {
        return World.FromJson(WorldUtilities.LoadTextFile(string.Format("Maps/{0}", worldName)));
    }

    public void CalculateNeighbors() //only blocks of same BlockType can be neighbors
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Block block = Blocks[i];
            IEnumerable<Block> blocksOfSameType = Blocks.Where(x => x.Info.BlockType == block.Info.BlockType);

            NeighborSet x = new NeighborSet()
            {
                Positive = blocksOfSameType.Where(b => b.Z == block.Z && b.Y == block.Y && b.X == block.X + (block.Info.Width / 2) + (b.Info.Width / 2)).FirstOrDefault(),
                Negative = blocksOfSameType.Where(b => b.Z == block.Z && b.Y == block.Y && b.X == block.X - (block.Info.Width / 2) - (b.Info.Width / 2)).FirstOrDefault(),
            };

            NeighborSet y = new NeighborSet()
            {
                Positive = blocksOfSameType.Where(b => b.X == block.X && b.Z == block.Z && b.Y == block.Y + (block.Info.Height / 2) + (b.Info.Height / 2)).FirstOrDefault(),
                Negative = blocksOfSameType.Where(b => b.X == block.X && b.Z == block.Z && b.Y == block.Y - (block.Info.Height / 2) - (b.Info.Height / 2)).FirstOrDefault(),
            };

            NeighborSet z = new NeighborSet()
            {
                Positive = blocksOfSameType.Where(b => b.X == block.X && b.Y == block.Y && b.Z == block.Z + (block.Info.Width / 2) + (b.Info.Width / 2)).FirstOrDefault(),
                Negative = blocksOfSameType.Where(b => b.X == block.X && b.Y == block.Y && b.Z == block.Z - (block.Info.Width / 2) - (b.Info.Width / 2)).FirstOrDefault(),
            };

            block.NeighborX = x;
            block.NeighborY = y;
            block.NeighborZ = z;
        }
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }
}
