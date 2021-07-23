using Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Block
{
    public int BlockInfoId;
    public int X;
    public int Y;
    public int Z;
    public Quaternion Rotation;
    public Vector3 RotationOffset;
    public int Id;

    private static int lastId = 1;

    public Vector3Int Position { get { return new Vector3Int(X, Y, Z); } }
    public Vector3 CenterPoint { get { return Position + new Vector3(Info.Width / 2f, Info.Height / 2f, Info.Width / 2f); } }

    public BlockInfo Info
    { 
        get { if (_info == null) { _info = BlockInfoLookup.Get(BlockInfoId); } return _info; }
        set { BlockInfoId = value.Id; _info = value; }
    }
    [NonSerialized]
    private BlockInfo _info;

    public NeighborSet NeighborX { get; set; }
    public NeighborSet NeighborY { get; set; }
    public NeighborSet NeighborZ { get; set; }

    public GameObject Instance { get; set; }
    public SeeThroughBlock SeeThroughBlock { get; set; }

    public Block(BlockInfo info, Vector3Int position)
    {
        _info = info;
        BlockInfoId = info.Id;
        X = position.X;
        Y = position.Y;
        Z = position.Z;
        Id = GenerateId();
    }

    public Block(BlockInfo info, int x, int y, int z)
    {
        _info = info;
        BlockInfoId = info.Id;
        X = x;
        Y = y;
        Z = z;
        Id = GenerateId();
    }

    public Block(int blockInfoId, int x, int y, int z)
    {
        _info = BlockInfoLookup.Get(blockInfoId);
        BlockInfoId = blockInfoId;
        X = x;
        Y = y;
        Z = z;
        Id = GenerateId();
    }

    private int GenerateId()
    {
        World.LastBlockId++;
        return World.LastBlockId;
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
        return Info.Prefab;
    }
}

public class NeighborSet
{
    public List<Block> Positive { get; set; }
    public List<Block> Negative { get; set; }
}
