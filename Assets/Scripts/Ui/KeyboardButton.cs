using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardButton : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI Text;

    public KeyboardButtonConfiguration ButtonConfiguration { get; set; }

    private string buttonText;

    public void Initialize(KeyboardButtonConfiguration buttonConfiguration, float buttonSize, float fontSize)
    {
        SetText(buttonConfiguration.Text);
        ButtonConfiguration = buttonConfiguration;

        Button.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonConfiguration.GetButtonWidth(buttonSize, fontSize), buttonSize);
    }

    private void SetText(string text)
    {
        buttonText = text;
        Text.text = text.ToString();
    }

    public void Shift()
    {
        SetText(buttonText.ToUpper());
    }

    public void UnShift()
    {
        SetText(buttonText.ToLower());
    }
}
