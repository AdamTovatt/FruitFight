using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagicLevelSettings
{
    public List<MagicLevelSettingsEntry> MagicLevels;

    public static Dictionary<int, MagicLevelSettingsEntry> Levels { get { if (_levels == null) Load(); return _levels; } private set { _levels = value; } }
    private static Dictionary<int, MagicLevelSettingsEntry> _levels;

    public static void Load()
    {
        try
        {
            _levels = new Dictionary<int, MagicLevelSettingsEntry>();
            MagicLevelSettings configuration = JsonUtility.FromJson<MagicLevelSettings>(WorldUtilities.LoadTextFile("Configuration/MagicLevelSettings"));

            foreach (MagicLevelSettingsEntry level in configuration.MagicLevels)
            {
                _levels.Add(level.Number, level);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Error when loading magic configuration! \n" + exception.Message);
        }
    }

    public static MagicLevelSettingsEntry GetSettingsForLevel(int levelNumber)
    {
        if(Levels.ContainsKey(levelNumber))
        {
            return Levels[levelNumber];
        }
        else
        {
            throw new Exception("Invalid magic level number: " + levelNumber);
        }
    }
}
