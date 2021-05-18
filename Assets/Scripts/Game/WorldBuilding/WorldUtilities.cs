using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilities
{
    public static string LoadTextFile(string path)
    {
        return Resources.Load<TextAsset>(path).text;
    }
}
