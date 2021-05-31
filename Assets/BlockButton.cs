using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockButton : MonoBehaviour
{
    public Image Image;
    public TextMeshProUGUI Text;
    public Button Button;

    public void Initialize(Texture2D image, string text)
    {
        Image.sprite = Sprite.Create(image, new Rect(new Vector2(0, 0), new Vector2(image.width, image.height)), new Vector2(image.width / 2, image.height / 2));
        Text.text = text;
    }
}
