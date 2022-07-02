using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileButton : MonoBehaviour
{
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI BodyText;

    public void Initialize(ProfileSave profile)
    {
        if(profile.EmptyProfile)
        {
            BodyText.text = "+";
        }
        else
        {
            BodyText.text = string.Format("{0} coins\n{1} xp\n{2}h played", profile.Coins, profile.Xp, profile.HoursPlayed);
        }

        HeaderText.text = profile.Name;
    }
}
