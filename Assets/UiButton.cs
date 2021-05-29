using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiButton : MonoBehaviour
{
    public Button ButtonComponent;
    public TextMeshProUGUI TextComponent;

    public void Initialize(string text, UiButton upButton, UiButton downButton)
    {
        Navigation navigation = new Navigation();
        if(upButton != null)
            navigation.selectOnUp = upButton.ButtonComponent;
        if (downButton != null) 
            navigation.selectOnDown = downButton.ButtonComponent;

        navigation.mode = Navigation.Mode.Explicit;

        ButtonComponent.navigation = navigation;
        TextComponent.text = text;
    }
}
