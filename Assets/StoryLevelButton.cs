using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLevelButton : MonoBehaviour
{
    public Button Button;
    public Image Thumbnail;
    public GameObject GreyOverlay;
    public TextMeshProUGUI Name;
    public UiSelectable Selectable;

    public void Initialize(string name, Sprite thumbnail, bool enabled)
    {
        Name.text = name;
        Thumbnail.sprite = thumbnail;

        if (enabled)
            Enable();
        else
            Disable();
    }

    public void Enable()
    {
        GreyOverlay.SetActive(false);
        Button.enabled = true;
        Selectable.enabled = true;
    }

    public void Disable()
    {
        GreyOverlay.SetActive(true);
        Button.enabled = false;
        Selectable.enabled = false;
    }

    public Sprite CreateScaledSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
