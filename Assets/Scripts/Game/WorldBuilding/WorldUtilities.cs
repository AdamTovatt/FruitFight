using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilities
{
    private static List<World> cachedStoryModeLevels;

    public static string LoadTextFile(string path)
    {
        return Resources.Load<TextAsset>(path).text;
    }

    public static List<World> GetStoryModeLevels()
    {
        if (cachedStoryModeLevels == null)
        {
            StoryModeConfiguration storyModeConfiguration = StoryModeConfiguration.LoadFromConfig();
            cachedStoryModeLevels = new List<World>();

            foreach (StoryModeLevelEntry level in storyModeConfiguration.Levels)
            {
                cachedStoryModeLevels.Add(World.FromWorldName(level.AssetName));
            }
        }

        return cachedStoryModeLevels;
    }

    public static void LoadStoryModeCache()
    {
        GetStoryModeLevels();
    }
}
