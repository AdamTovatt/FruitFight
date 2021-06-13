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
        if (axis.Y > 0)
            sideLength = Info.Height;
        else
            sideLength = Info.Width;

        Vector3Int otherAxis = new Vector3Int(Math.Abs(axis.X - 1), 0, Math.Abs(axis.Z - 1));

        Vector3Int distance = axis * sideLength;
        Vector3Int positive = position + distance + axis;
        Vector3Int negative = position - axis;

        List<Block> positiveBlocks = new List<Block>();
        List<Block> negativeBlocks = new List<Block>();

        if (axis.Y == 0)
        {
            for (int i = 0; i < sideLength; i++)
            {
                Vector3Int checkPosition = positive + i * otherAxis;
                positiveBlocks.AddRange(world.GetBlocksAtPosition(checkPosition));
                negativeBlocks.AddRange(world.GetBlocksAtPosition(negative + i * otherAxis));
            }
        }
        else
        {
            positiveBlocks.AddRange(world.GetBlocksAtPosition(positive));
            negativeBlocks.AddRange(world.GetBlocksAtPosition(negative));
        }

        return new NeighborSet()
        {
            Positive = positiveBlocks.Where(b => b.Info.BlockType == _info.BlockType).ToList(),
            Negative = negativeBlocks.Where(b => b.Info.BlockType == _info.BlockType).ToList(),
        };
    }

    public override string ToString()
    {
        return _info.Prefab;
    }
}

public class NeighborSet
{
    public List<Block> Positive { get; set; }
    public List<Block> Negative { get; set; }
}
