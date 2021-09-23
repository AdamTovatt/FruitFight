using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadLevelRequestBody
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("thumbnail")]
    public string Thumbnail { get; set; }

    [JsonProperty("worldData")]
    public string WorldData { get; set; }

    [JsonProperty("twoPlayes")]
    public bool twoPlayers { get; set; }

    [JsonProperty("puzzleRatio")]
    public int PuzzleRatio { get; set; }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
