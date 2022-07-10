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
            BodyText.text = profile.ToString();
        }

        HeaderText.text = profile.Name;
    }
}
