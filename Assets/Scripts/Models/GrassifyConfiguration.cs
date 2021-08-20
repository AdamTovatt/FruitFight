using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GrassifyConfiguration
{
    public List<int> GrowBlocks;
    public List<GrassifyBlockConfiguration> VegetationBlocks;

    public static GrassifyConfiguration LoadFromConfig()
    {
        return JsonUtility.FromJson<GrassifyConfiguration>(WorldUtilities.LoadTextFile("Configuration/GrassifyConfiguration"));
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }
}

[Serializable]
public class GrassifyBlockConfiguration
{
    public int Id;
    public float Probability;
    public List<int> Variations;
    public float VariationProbability;
    public bool AllowOverlap = false;

    public GrassifyBlockConfiguration() { }
}
