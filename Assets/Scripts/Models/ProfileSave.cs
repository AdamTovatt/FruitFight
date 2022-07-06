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
}
