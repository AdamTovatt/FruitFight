using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IconConfiguration
{
    public List<IconConfigurationEntry> Icons;

    public static IconConfiguration LoadFromConfiguration()
    {
        return JsonUtility.FromJson<IconConfiguration>(WorldUtilities.LoadTextFile("Configuration/IconConfiguration"));
    }
}

[Serializable]
public class IconConfigurationEntry
{
    public string Name;
    public bool VaryByDevice;
    public string FileName;
    public string FileNameKeyboard;
    public string FileNameController;
}