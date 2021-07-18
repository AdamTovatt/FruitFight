using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public float FontSize = 40f;
    public float ButtonSize = 70f;
    public float ButtonSpacing = 10f;

    private RectTransform rectTransform;

    private List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
    private static KeyboardConfiguration keyboardConfiguration;

    void Start()
    {
        if (keyboardConfiguration == null)
            keyboardConfiguration = KeyboardConfiguration.LoadFromConfiguration();

        rectTransform = gameObject.GetComponent<RectTransform>();

        float keyboardWidth = 0;
        List<float> rowsWidths = new List<float>();
        foreach (KeyboardButtonRow row in keyboardConfiguration.ButtonRows)
        {
            float rowWidth = ButtonSpacing;
            foreach (KeyboardButtonConfiguration button in row.Buttons)
            {
                rowWidth += button.GetButtonWidth(ButtonSize, FontSize) + ButtonSpacing;
            }
            rowsWidths.Add(rowWidth);
        }
        keyboardWidth = rowsWidths.OrderByDescending(f => f).First();

        float keyboardHeight = keyboardConfiguration.ButtonRows.Count * ButtonSize + keyboardConfiguration.ButtonRows.Count * ButtonSpacing;

        rectTransform.sizeDelta = new Vector2(keyboardWidth + (ButtonSpacing * 2), keyboardHeight + (ButtonSpacing * 2));

        for (int y = 0; y < keyboardConfiguration.ButtonRows.Count; y++)
        {
            buttons.Add(new List<KeyboardButton>());

            float sumPositionX = 0;
            for (int x = 0; x < keyboardConfiguration.ButtonRows[(keyboardConfiguration.ButtonRows.Count - 1) - y].Buttons.Count; x++)
            {
                GameObject button = Instantiate(ButtonPrefab, transform);
                button.transform.localPosition = new Vector3(sumPositionX - keyboardWidth / 2, (y * ButtonSize + y * ButtonSpacing) - keyboardHeight / 2);

                KeyboardButton keyboardButton = button.GetComponent<KeyboardButton>();
                KeyboardButtonConfiguration buttonConfiguration = keyboardConfiguration.ButtonRows[(keyboardConfiguration.ButtonRows.Count - 1) - y].Buttons[x];
                keyboardButton.Initialize(buttonConfiguration, ButtonSize, FontSize);
                keyboardButton.Button.onClick.AddListener(() => ButtonWasClicked(keyboardButton));

                sumPositionX += buttonConfiguration.GetButtonWidth(ButtonSize, FontSize) + ButtonSpacing;

                buttons[y].Add(keyboardButton);
            }
        }
    }

    public void ButtonWasClicked(KeyboardButton button)
    {

    }

    void Update()
    {

    }
}
