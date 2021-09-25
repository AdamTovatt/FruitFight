using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI LevelText;
    public Image LevelImage;
    public Button Button;

    public Sprite CreateScaledSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    public void Init(string title, string levelText, Sprite thumbnail)
    {
        Title.text = title;
        LevelText.text = levelText;
        LevelImage.sprite = thumbnail;
    }
}
