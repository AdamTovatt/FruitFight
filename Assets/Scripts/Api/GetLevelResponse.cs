using Newtonsoft.Json;

public class GetLevelResponse
{
    [JsonProperty("worldData")]
    public string WorldData { get; set; }

    [JsonProperty("metaData")]
    public WorldMetadata Metadata { get; set; }

    [JsonProperty("likingUserId")]
    public long? LikingUserId { get; set; }

    public GetLevelResponse() { }

    public static GetLevelResponse FromJson(string json)
    {
        return JsonConvert.DeserializeObject<GetLevelResponse>(json);
    }
}
