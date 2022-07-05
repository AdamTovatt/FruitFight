using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLevelButton : MonoBehaviour
{
    public Button Button;
    public Image Thumbnail;
    public TextMeshProUGUI Name;

    public void Initialize(string name, Sprite thumbnail)
    {
        Name.text = name;
        Thumbnail.sprite = thumbnail;
    }

    public Sprite CreateScaledSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
