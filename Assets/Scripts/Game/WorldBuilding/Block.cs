using Lookups;
using System;

[Serializable]
public class Block
{
    public int BlockInfoId;
    public int X;
    public int Y;
    public int Z;

    public Vector3Int Position { get { return new Vector3Int(X, Y, Z); } }

    public BlockInfo Info { get { if (_info == null) { _info = BlockInfoLookup.Get(BlockInfoId); } return _info; } }
    [NonSerialized]
    private BlockInfo _info;

    public NeighborSet NeighborX { get; set; }
    public NeighborSet NeighborY { get; set; }
    public NeighborSet NeighborZ { get; set; }

    public Block(BlockInfo info, Vector3Int position)
    {
        _info = info;
        X = position.X;
        Y = position.Y;
        Z = position.Z;
    }

    public Block(BlockInfo info, int x, int y, int z)
    {
        _info = info;
        X = x;
        Y = y;
        Z = z;
    }

    public Block(int blockInfoId, int x, int y, int z)
    {
        _info = BlockInfoLookup.Get(blockInfoId);
        X = x;
        Y = y;
        Z = z;
    }

    public void FetchBlockInfo()
    {
        if (_info == null)
        {
            _info = BlockInfoLookup.Get(BlockInfoId);
        }
    }
}

public class NeighborSet
{
    public Block Positive { get; set; }
    public Block Negative { get; set; }
}
