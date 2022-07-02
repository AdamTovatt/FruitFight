using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveProfileHelper
{
    public static ProfileSaveState GetSaveState()
    {
        return new ProfileSaveState()
        {
            Profiles = new List<ProfileSave>()
            {
                new ProfileSave("herman")
                {
                    Coins = 124,
                    Xp = 1200,
                    HoursPlayed = 42
                }
            }
        };
    }
}
