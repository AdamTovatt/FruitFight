using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiPlayerInLobby : MonoBehaviour
{
    public TextMeshProUGUI NameText;

    public void SetName(string text)
    {
        NameText.text = text;
    }
}
