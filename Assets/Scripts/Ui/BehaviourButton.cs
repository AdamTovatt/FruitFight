using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourButton : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Button Button;
    public Image BackgroundImage;
    public Color AddedColor;
    public Color AvailableColor;

    public void Initialize(string text, bool hasAddedBehaviour)
    {
        Text.text = text.Split("Properties")[0];
        BackgroundImage.color = hasAddedBehaviour ? AddedColor : AvailableColor;
    }
}
