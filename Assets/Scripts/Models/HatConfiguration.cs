using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HatConfiguration
{
    public List<HatConfigurationEntry> HatList;

    public static Dictionary<int, HatConfigurationEntry> Hats { get { if (_hats == null) Load(); return _hats; } private set { _hats = value; } }
    private static Dictionary<int, HatConfigurationEntry> _hats;

    public static void Load()
    {
        try
        {
            _hats = new Dictionary<int, HatConfigurationEntry>();
            HatConfiguration configuration = JsonUtility.FromJson<HatConfiguration>(WorldUtilities.LoadTextFile("Configuration/HatConfiguration"));

            foreach (HatConfigurationEntry hat in configuration.HatList)
            {
                _hats.Add(hat.Id, hat);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Error when loading hat configuration! \n" + exception.Message);
        }
    }

    public static GameObject GetHatPrefab(int hatId)
    {
        if (hatId == 0) //0 is no hat
            return null;

        if(Hats.ContainsKey(hatId))
        {
            if (Hats[hatId].Prefab != null)
                return Hats[hatId].Prefab;
        }

        Debug.LogError("Error when getting hat prefab: " + hatId);
        return null;
    }
}
