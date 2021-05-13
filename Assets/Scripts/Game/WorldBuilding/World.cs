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
        world.CalculateNeighborsA();
        return world;
    }

    private void CalculateNeighborsA()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Block block = Blocks[i];
            IEnumerable<Block> blocksOfSameType = Blocks.Where(x => x.Info.BlockType == block.Info.BlockType);
        
            NeighborSet x = new NeighborSet();
            NeighborSet y = new NeighborSet();
            NeighborSet z = new NeighborSet();

            foreach (Block otherBlock in blocksOfSameType)
            {
                int requiredHorizontalDistance = (otherBlock.Info.Width / 2) + (block.Info.Width / 2);
                int requiredVerticalDistance = (otherBlock.Info.Height / 2) + (block.Info.Height / 2);

                int distanceX = otherBlock.X - block.X;
                int distanceY = otherBlock.Y - block.Y;
                int distanceZ = otherBlock.Z - block.Z;

                if (distanceX == requiredHorizontalDistance)
                    x.Positive = otherBlock;
                else if (distanceX == requiredHorizontalDistance * -1)
                    x.Negative = otherBlock;
                else if (distanceY == requiredHorizontalDistance)
                    y.Positive = otherBlock;
                else if (distanceY == requiredHorizontalDistance * -1)
                    y.Negative = otherBlock;
                else if (distanceZ == requiredVerticalDistance)
                    z.Positive = otherBlock;
                else if (distanceZ == requiredVerticalDistance * -1)
                    z.Negative = otherBlock;
            }           

            block.NeighborX = x;
            block.NeighborY = y;
            block.NeighborZ = z;
        }
    }

    private void CalculateNeighborsB()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Block block = Blocks[i];
            IEnumerable<Block> blocksOfSameType = Blocks.Where(x => x.Info.BlockType == block.Info.BlockType);

            NeighborSet x = new NeighborSet()
            {
                Positive = blocksOfSameType.Where(b => b.X == block.X + (block.Info.Width / 2) + (b.Info.Width / 2)).FirstOrDefault(),
                Negative = blocksOfSameType.Where(b => b.X == block.X - (block.Info.Width / 2) - (b.Info.Width / 2)).FirstOrDefault(),
            };

            NeighborSet y = new NeighborSet()
            {
                Positive = blocksOfSameType.Where(b => b.Y == block.Y + (block.Info.Height / 2) + (b.Info.Height / 2)).FirstOrDefault(),
                Negative = blocksOfSameType.Where(b => b.Y == block.Y - (block.Info.Height / 2) - (b.Info.Height / 2)).FirstOrDefault(),
            };

            NeighborSet z = new NeighborSet()
            {
                Positive = blocksOfSameType.Where(b => b.Z == block.Z + (block.Info.Width / 2) + (b.Info.Width / 2)).FirstOrDefault(),
                Negative = blocksOfSameType.Where(b => b.Z == block.Z - (block.Info.Width / 2) - (b.Info.Width / 2)).FirstOrDefault(),
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
