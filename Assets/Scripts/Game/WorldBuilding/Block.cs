using System;
using UnityEngine;

[Serializable]
public class Block
{
    public int BlockInfoId;
    public int X;
    public int Y;
    public int Z;

    public BlockInfo Info { get { if (_info == null) { _info = WorldBuilder.GetBlockInfo(BlockInfoId); } return _info; } }
    [NonSerialized]
    private BlockInfo _info;

    public NeighborSet NeighborX { get; set; }
    public NeighborSet NeighborY { get; set; }
    public NeighborSet NeighborZ { get; set; }

    public Block(BlockInfo info, int x, int y, int z)
    {
        _info = info;
        X = x;
        Y = y;
        Z = z;
    }

    public Block(int blockInfoId, int x, int y, int z)
    {
        _info = WorldBuilder.GetBlockInfo(blockInfoId);
        X = x;
        Y = y;
        Z = z;
    }

    public void FetchBlockInfo()
    {
        if (_info == null)
        {
            _info = WorldBuilder.GetBlockInfo(BlockInfoId);
        }
    }
}

public class NeighborSet
{
    public Block Positive { get; set; }
    public Block Negative { get; set; }
}
