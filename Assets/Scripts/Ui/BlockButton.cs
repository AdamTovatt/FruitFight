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

    public BlockInfo BlockInfo { get; set; }
    public int MenuIndex { get; set; }

    public void Initialize(Texture2D image, string text)
    {
        Image.sprite = Sprite.Create(image, new Rect(new Vector2(0, 0), new Vector2(image.width, image.height)), new Vector2(image.width / 2, image.height / 2));
        Text.text = text;
    }

    public void SetNavigation(Button up, Button down, Button left, Button right)
    {
        Navigation navigation = new Navigation();
        navigation.selectOnUp = up;
        navigation.selectOnDown = down;
        navigation.selectOnLeft = left;
        navigation.selectOnRight = right;
        navigation.mode = Navigation.Mode.Explicit;
        Button.navigation = navigation;
    }

    public override string ToString()
    {
        return Text == null ? "null" : Text.text;
    }
}
