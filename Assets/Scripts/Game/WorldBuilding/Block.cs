using System;
using UnityEngine;

[Serializable]
public class Block
{
    public BlockInfo Info;
    public int X;
    public int Y;
    public int Z;

    public NeighborSet NeighborX { get; set; }
    public NeighborSet NeighborY { get; set; }
    public NeighborSet NeighborZ { get; set; }

    public Block(BlockInfo info, int x, int y, int z)
    {
        Info = info;
        X = x;
        Y = y;
        Z = z;
    }
}

public class NeighborSet
{
    public Block Positive { get; set; }
    public Block Negative { get; set; }
}
