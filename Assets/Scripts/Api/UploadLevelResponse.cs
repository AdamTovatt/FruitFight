using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadLevelResponse
{
    public long LevelId { get; set; }
    public ErrorResponse ErrorResponse { get; set; }

    public UploadLevelResponse() { }

    public static UploadLevelResponse FromJson(string json)
    {
        return JsonConvert.DeserializeObject<UploadLevelResponse>(json);
    }
}
