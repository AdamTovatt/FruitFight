using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldMetadata
{
    public string Name;
    public string Description;
    public string ImageData;
    public int AuthorId;
    public string AuthorName;

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

    public Texture2D GetImageDataAsTexture2d()
    {
        Texture2D result = new Texture2D(512, 512);
        result.LoadImage(Convert.FromBase64String(ImageData));
        return result;
    }
}

[Serializable]
public class WorldMetadataResponse
{
    public List<WorldMetadata> Levels;

    public WorldMetadataResponse() { }

    public static WorldMetadataResponse FromJson(string json)
    {
        return JsonConvert.DeserializeObject<WorldMetadataResponse>(json);
    }
}
