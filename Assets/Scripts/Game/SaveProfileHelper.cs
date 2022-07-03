using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveProfileHelper
{
    private static string saveStatePath = Path.Combine(Application.persistentDataPath, "profileSaveState.json");

    public static ProfileSaveState GetSaveState()
    {
        EnsureSaveStateFileExistance();

        string saveStateJson = null;

        try
        {
            saveStateJson = File.ReadAllText(saveStatePath);
        }
        catch (Exception)
        {
            throw new Exception("Error when reading save state file");
        }

        try
        {
            return ProfileSaveState.FromJson(saveStateJson);
        }
        catch (Exception)
        {
            throw new Exception("Error when parsing save state file");
        }
    }

    public static void WriteSaveState(ProfileSaveState state)
    {
        try
        {
            File.WriteAllText(saveStatePath, state.ToJson());
        }
        catch (Exception)
        {
            throw new Exception("Error when writing save state file");
        }
    }

    private static void EnsureSaveStateFileExistance()
    {
        if (!File.Exists(saveStatePath))
        {
            ProfileSaveState save = ProfileSaveState.Empty;
            File.WriteAllText(saveStatePath, save.ToJson());
        }
    }
}
