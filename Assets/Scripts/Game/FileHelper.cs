using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileHelper
{
    private const string mapDirectoryBaseString = "{0}/maps";
    private const string userCredentialsBaseString = "{0}/userCredentials.json";

    public static List<WorldMetadata> LoadMetadatasFromDisk()
    {
        List<WorldMetadata> worldMetadatas = new List<WorldMetadata>();

        string mapDirectory = GetPath(mapDirectoryBaseString);

        if (!Directory.Exists(mapDirectory))
            Directory.CreateDirectory(mapDirectory);

        foreach (string file in Directory.GetFiles(mapDirectory).Where(x => x.EndsWith(".mapmeta")).ToList())
        {
            worldMetadatas.Add(WorldMetadata.FromJson(File.ReadAllText(file)));
        }

        return worldMetadatas;
    }

    public static string LoadMapData(string levelName)
    {
        string mapDirectory = GetPath(mapDirectoryBaseString);

        if (!Directory.Exists(mapDirectory))
            return null;

        string filePath = Path.Combine(mapDirectory, string.Format("{0}.mapdata", levelName.Replace(' ', '_')));
        
        if (!File.Exists(filePath))
            return null;

        return File.ReadAllText(filePath);
    }

    public static UserCredentials LoadUserCredentials()
    {
        string credentialsPath = GetPath(userCredentialsBaseString);

        if(File.Exists(credentialsPath))
        {
            return UserCredentials.FromJson(File.ReadAllText(credentialsPath));
        }

        return null;
    }

    public static void SaveUserCredentials(UserCredentials userCredentials)
    {
        string credentialspath = GetPath(userCredentialsBaseString);

        File.WriteAllText(credentialspath, userCredentials.ToJson());
    }

    private static string GetPath(string baseString)
    {
        return string.Format(baseString, Application.persistentDataPath);
    }
}
