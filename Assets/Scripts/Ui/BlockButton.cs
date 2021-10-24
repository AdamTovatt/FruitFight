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

    public delegate void OnSelectHandler(BlockButton button);
    public event OnSelectHandler OnSelected;

    public BlockInfo BlockInfo { get; set; }
    public int MenuIndex { get; set; }

    public UiSelectable Selectable { get; private set; }

    public void Initialize(Texture2D image, string text)
    {
        Image.sprite = Sprite.Create(image, new Rect(new Vector2(0, 0), new Vector2(image.width, image.height)), new Vector2(image.width / 2, image.height / 2));
        Text.text = text;

        Selectable = gameObject.GetComponent<UiSelectable>();
        if (Selectable != null)
            Selectable.OnSelected += SelectedComponentWasSelected;
    }

    private void SelectedComponentWasSelected()
    {
        OnSelected?.Invoke(this);
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

    public void OnDestroy()
    {
        OnSelected = null;
    }

    public override string ToString()
    {
        return Text == null ? "null" : Text.text;
    }
}
