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
                World world = World.FromWorldName(level.AssetName);
                world.StoryModeLevelEntry = level;
                cachedStoryModeLevels.Add(world);
            }
        }

        return cachedStoryModeLevels;
    }

    public static void LoadStoryModeCache()
    {
        GetStoryModeLevels();
    }
}
