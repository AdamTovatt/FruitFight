using Assets.Scripts.Models;
using Lookups;
using Newtonsoft.Json;
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
    public bool IsFromGrassify;
    public bool HasPropertyExposer;
    public BehaviourPropertyContainer BehaviourProperties;
    public List<BehaviourProperties> BehaviourProperties2;

    [JsonIgnore]
    public Vector3 AppliedPosition { get { return new Vector3(X, Y, Z) + RotationOffset; } }
    [JsonIgnore]
    public Vector3Int Position { get { return new Vector3Int(X, Y, Z); } }
    [JsonIgnore]
    public Vector3 CenterPoint { get { return Position + new Vector3(Info.Width / 2f, Info.Height / 2f, Info.Width / 2f); } }

    public bool EnforceEdge { get; set; }

    [JsonIgnore]
    public BlockInfo Info
    {
        get { if (_info == null) { _info = BlockInfoLookup.Get(BlockInfoId); } return _info; }
        set { BlockInfoId = value.Id; _info = value; }
    }
    private BlockInfo _info;

    [JsonIgnore]
    public NeighborSet NeighborX { get; set; }
    [JsonIgnore]
    public NeighborSet NeighborY { get; set; }
    [JsonIgnore]
    public NeighborSet NeighborZ { get; set; }

    [JsonIgnore]
    public GameObject Instance { get; set; }
    [JsonIgnore]
    public BlockInformationHolder InformationHolder { get; set; }
    [JsonIgnore]
    public SeeThroughBlock SeeThroughBlock { get; set; }

    [JsonIgnore]
    public List<Vector3Int> ObscuredPositions { get; set; }

    public Block() { }

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

    public void MakeEnforceEdges()
    {
        EnforceEdge = true;

        foreach (Block neighbor in NeighborX.SameTypesPositive)
            neighbor.EnforceEdge = true;
        foreach (Block neighbor in NeighborX.SameTypesNegative)
            neighbor.EnforceEdge = true;
        foreach (Block neighbor in NeighborY.SameTypesPositive)
            neighbor.EnforceEdge = true;
        foreach (Block neighbor in NeighborY.SameTypesNegative)
            neighbor.EnforceEdge = true;
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
        Vector3Int xUnit = new Vector3Int(1, 0, 0);
        Vector3Int zUnit = new Vector3Int(0, 0, 1);

        Vector3Int distance = axis * sideLength;
        Vector3Int positive = position + distance + axis; //add axis since origin point is at bottom of block and we want to start looking for neighbors at the top of the block
        Vector3Int negative = position - axis;

        List<Block> positiveBlocks = new List<Block>();
        List<Block> negativeBlocks = new List<Block>();

        if (axis.Y == 0)
        {
            for (int i = 0; i < sideLength; i++)
            {
                positiveBlocks.AddRange(world.GetBlocksAtPosition(positive + i * otherAxis));
                negativeBlocks.AddRange(world.GetBlocksAtPosition(negative + i * otherAxis));
            }
        }
        else
        {
            for (int x = 0; x < Info.Width; x++)
            {
                for (int z = 0; z < Info.Width; z++)
                {
                    for (int y = 0; y < Info.Height; y++)
                    {
                        positiveBlocks.AddRange(world.GetBlocksAtPosition(position + distance + xUnit * x + zUnit * z + axis * y));
                        negativeBlocks.AddRange(world.GetBlocksAtPosition(negative + xUnit * x + zUnit * z - axis * y));
                    }
                }
            }
        }

        return new NeighborSet()
        {
            SameTypesPositive = positiveBlocks.Where(b => b.Info.BlockType == _info.BlockType && b.Id != Id).ToList(), //if the current block is BlockType.Block
            SameTypesNegative = negativeBlocks.Where(b => b.Info.BlockType == _info.BlockType && b.Id != Id).ToList(), //only return blocks of same type, else return all blocks
            AllTypesPositive = positiveBlocks.Where(b => b.Id != Id).ToList(),
            AllTypesNegative = negativeBlocks.Where(b => b.Id != Id).ToList(),
        };
    }

    public bool HasNeighborWithId(int id)
    {
        return NeighborX.HasNeighborWithId(id) || NeighborY.HasNeighborWithId(id) || NeighborZ.HasNeighborWithId(id);
    }

    public override string ToString()
    {
        return Info.Prefab;
    }
}

public class NeighborSet
{
    public bool Exists { get { return SameTypesPositive.Count > 0 || SameTypesNegative.Count > 0; } }
    public int OccupiedSides { get { int count = 0; if (SameTypesPositive.Count > 0) count++; if (SameTypesNegative.Count > 0) count++; return count; } }
    public List<Block> SameTypesPositive { get; set; }
    public List<Block> SameTypesNegative { get; set; }
    public List<Block> AllTypesPositive { get; set; }
    public List<Block> AllTypesNegative { get; set; }

    public bool HasNeighborWithId(int id)
    {
        foreach (Block block in AllTypesPositive)
        {
            if (block.BlockInfoId == id)
                return true;
        }

        foreach (Block block in AllTypesNegative)
        {
            if (block.BlockInfoId == id)
                return true;
        }

        return false;
    }
}
