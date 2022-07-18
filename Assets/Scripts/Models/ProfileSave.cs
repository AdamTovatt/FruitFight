using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSave
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("hoursPlayed")]
    public double HoursPlayed { get; set; }

    [JsonProperty("xp")]
    public double Xp { get; set; }

    [JsonProperty("coins")]
    public int Coins { get; set; }

    [JsonProperty("jellyBeans")]
    public int JellyBeans { get; set; }

    [JsonProperty("emptyState")]
    public bool EmptyProfile { get; set; }

    [JsonProperty]
    public List<int> CompletedLevelIds { get; set; }

    [JsonIgnore]
    public static ProfileSave Empty { get { return new ProfileSave("(empty)") { EmptyProfile = true }; } }

    public ProfileSave() { }

    public ProfileSave(string name)
    {
        Name = name;
    }

    public bool HasCompletedLevel(int id)
    {
        if (CompletedLevelIds == null)
            return false;

        return CompletedLevelIds.Contains(id);
    }

    public void AddCompletedLevel(int id)
    {
        if (CompletedLevelIds == null)
            CompletedLevelIds = new List<int>();

        if (!CompletedLevelIds.Contains(id))
            CompletedLevelIds.Add(id);
    }

    public bool HasUnlockedLevel(StoryModeLevelEntry level, out string buttonText)
    {
        buttonText = null;
        bool buttonEnabled = level.Id == 1 || HasCompletedLevel(level.Id - 1);

        if (level.RequiredXp > 0)
        {
            if (Xp < level.RequiredXp)
            {
                buttonEnabled = false;

                buttonText = string.Format("{0}xp required to unlock", level.RequiredXp);
            }
        }

        return buttonEnabled;
    }

    public override string ToString()
    {
        return string.Format("{0} coins\n{1} xp\n{2}h played", Coins, Xp, (Mathf.CeilToInt((float)HoursPlayed * 10f) / 10.0f).ToString("0.0"));
    }
}
