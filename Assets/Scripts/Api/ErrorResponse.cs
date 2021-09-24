using Newtonsoft.Json;

public class ErrorResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; }

    public static ErrorResponse FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ErrorResponse>(json);
    }
}
