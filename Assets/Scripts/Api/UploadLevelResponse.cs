using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class UploadLevelResponse
{
    public long LevelId { get; set; }
    public ErrorResponse ErrorResponse { get; set; }
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public UploadLevelResponse() { }

    public static UploadLevelResponse FromJson(string json)
    {
        return JsonConvert.DeserializeObject<UploadLevelResponse>(json);
    }

    public UploadLevelResponse(string json, bool success, HttpStatusCode statusCode)
    {
        if(success)
        {
            UploadLevelResponse response = JsonConvert.DeserializeObject<UploadLevelResponse>(json);
            LevelId = response.LevelId;
        }
        else
        {
            ErrorResponse = ErrorResponse.FromJson(json);
        }

        Success = success;
        StatusCode = StatusCode;
    }
}
