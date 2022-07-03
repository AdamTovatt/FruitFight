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

    [JsonIgnore]
    public static ProfileSave Empty { get { return new ProfileSave("(empty)") { EmptyProfile = true }; } }

    public ProfileSave() { }

    public ProfileSave(string name)
    {
        Name = name;
    }
}
