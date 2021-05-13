using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockInfo
{
    public int Id;
    public BlockType BlockType;
    public int Width;
    public int Height;
    public string Prefab;
    public List<string> EdgePrefabs;

    public BlockInfo()
    {
        EdgePrefabs = new List<string>();
    }
}

public enum BlockType
{
    Large, Invisible
}

[Serializable]
public class BlockInfoContainer
{
    public List<BlockInfo> Infos;

    public BlockInfoContainer()
    {
        Infos = new List<BlockInfo>();
    }
}