using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabConfiguration
{
    [JsonProperty("prefabs")]
    public List<PrefabConfigurationEntry> Prefabs { get; set; }

    private static PrefabConfiguration instance;

    public static PrefabConfiguration GetInstance()
    {
        if(instance == null)
        {
            try
            {
                instance = JsonConvert.DeserializeObject<PrefabConfiguration>(WorldUtilities.LoadTextFile("Configuration/PrefabConfiguration"));
            }
            catch (Exception exception)
            {
                Debug.LogError("Error when loading prefab configuration: " + exception.Message);
            }
        }

        return instance;
    }
}
