using System;
using UnityEngine;

[Serializable]
public class WorldMetadata
{
    public string Name;

    public WorldMetadata() { }
    public WorldMetadata(string name)
    {
        Name = name;
    }

    public static WorldMetadata FromJson(string json)
    {
        return JsonUtility.FromJson<WorldMetadata>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
