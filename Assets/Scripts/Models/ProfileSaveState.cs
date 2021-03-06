using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSaveState
{
    [JsonProperty("profiles")]
    public List<ProfileSave> Profiles { get; set; }

    [JsonIgnore]
    public static ProfileSaveState Empty { get { return GetEmptyState(); } }
    
    public ProfileSaveState()
    {
        Profiles = new List<ProfileSave>();
    }

    public void AddProfile(ProfileSave profileSave)
    {
        if (Profiles.Count >= 3)
            throw new System.Exception("Cannot add profile to a full profile state, this state already contains 3 profiles");

        Profiles.Add(profileSave);
    }
    
    public void RemoveProfile(int index)
    {
        Profiles[index] = ProfileSave.Empty;
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
            return ProfileSave.Empty;

        return Profiles[index];
    }

    private static ProfileSaveState GetEmptyState()
    {
        ProfileSaveState state = new ProfileSaveState();

        for (int i = 0; i < 3; i++)
        {
            state.AddProfile(ProfileSave.Empty);
        }

        return state;
    }
}
