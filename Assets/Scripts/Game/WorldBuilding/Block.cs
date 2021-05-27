using Lookups;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public void CalculateNeighbors(World world)
    {
        Vector3Int position = Position;
        NeighborX = CalculateNeighborSet(world, new Vector3Int(1, 0, 0), position);
        NeighborY = CalculateNeighborSet(world, new Vector3Int(0, 1, 0), position);
        NeighborZ = CalculateNeighborSet(world, new Vector3Int(0, 0, 1), position);
    }

    private NeighborSet CalculateNeighborSet(World world, Vector3Int axis, Vector3Int position)
    {
        int sideLength = 0;
        if (axis.Z > 0)
            sideLength = Info.Height;
        else
            sideLength = Info.Width;

        Vector3Int distance = axis * sideLength;
        Vector3Int positive = position + distance;
        Vector3Int negative = position - distance;

        List<Block> positiveBlocks = world.GetBlocksAtPosition(positive);
        List<Block> negativeBlocks = world.GetBlocksAtPosition(negative);

        return new NeighborSet()
        {
            Positive = positiveBlocks.Where(b => b.Info.BlockType == _info.BlockType).FirstOrDefault(),
            Negative = negativeBlocks.Where(b => b.Info.BlockType == _info.BlockType).FirstOrDefault(),
        };
    }
}

public class NeighborSet
{
    public Block Positive { get; set; }
    public Block Negative { get; set; }
}
