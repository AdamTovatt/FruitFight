using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabConfigurationEntry
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    public override string ToString()
    {
        if (Name == null)
            return Path == null ? "(Invalid name and path)" : Path;

        return Name;
    }
}
