using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileHelper
{
    public static List<WorldMetadata> LoadMetadatasFromDisk()
    {
        List<WorldMetadata> worldMetadatas = new List<WorldMetadata>();

        string mapDirectory = string.Format("{0}/maps", Application.persistentDataPath);

        if (!Directory.Exists(mapDirectory))
            Directory.CreateDirectory(mapDirectory);

        foreach (string file in Directory.GetFiles(mapDirectory).Where(x => x.EndsWith(".mapmeta")).ToList())
        {
            worldMetadatas.Add(WorldMetadata.FromJson(File.ReadAllText(file)));
        }

        return worldMetadatas;
    }
}
