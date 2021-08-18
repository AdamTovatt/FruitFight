using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GrassifyConfiguration
{
    public List<int> GrowBlocks;
    public List<GrassifyBlockConfiguration> VegetationBlocks;

    public static GrassifyBlockConfiguration FromJson()
    {
        return JsonUtility.FromJson<GrassifyBlockConfiguration>(WorldUtilities.LoadTextFile("Configuration/GrassifyConfiguration"));
    }
}
