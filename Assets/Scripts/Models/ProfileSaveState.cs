using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSaveState
{
    [JsonProperty("profiles")]
    public List<ProfileSave> Profiles { get; set; }

    public void AddProfile(ProfileSave profileSave)
    {
        if (Profiles.Count >= 3)
            throw new System.Exception("Cannot add profile to a full profile state, this state already contains 3 profiles");

        Profiles.Add(profileSave);
    }
    
    public static ProfileSaveState FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ProfileSaveState>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public ProfileSave GetProfile(int index)
    {
        if (!(Profiles.Count > index))
            return new ProfileSave("(empty)") { EmptyProfile = true };

        return Profiles[index];
    }
}
