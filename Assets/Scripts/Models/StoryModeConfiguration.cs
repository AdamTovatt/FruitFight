using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryModeConfiguration
{
    [JsonProperty("levels")]
    public List<StoryModeLevelEntry> Levels;

    public static StoryModeConfiguration LoadFromConfig()
    {
        return JsonConvert.DeserializeObject<StoryModeConfiguration>(WorldUtilities.LoadTextFile("Configuration/StoryModeConfiguration"));
    }
}

public class StoryModeLevelEntry
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("assetName")]
    public string AssetName { get; set; }
}
