using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public float FontSize = 40f;
    public float ButtonSize = 70f;
    public float ButtonSpacing = 10f;
    public TMP_InputField TextField;

    private RectTransform rectTransform;

    private List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
    private static KeyboardConfiguration keyboardConfiguration;

    private static StringBuilder currentText;

    public delegate void TextSubmittedHandler(object sender, string text);
    public event TextSubmittedHandler OnTextSubmitted;

    public delegate void ClosedHandler(object sender);
    public event ClosedHandler OnClosed;

    private bool isEnteringTextWithKeyboard = false;
    private PlayerControls inputActions;

    private bool isShifted;

    void Start()
    {
        currentText = new StringBuilder();

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
        keyboardWidth = rowsWidths.OrderByDescending(f => f).First() + (ButtonSpacing * 2);

        float keyboardHeight = (keyboardConfiguration.ButtonRows.Count * ButtonSize + keyboardConfiguration.ButtonRows.Count * ButtonSpacing) + (ButtonSpacing * 2);

        rectTransform.sizeDelta = new Vector2(keyboardWidth, keyboardHeight);
        TextField.GetComponent<RectTransform>().sizeDelta = new Vector2(keyboardWidth, TextField.GetComponent<RectTransform>().sizeDelta.y);

        for (int y = 0; y < keyboardConfiguration.ButtonRows.Count; y++)
        {
            buttons.Add(new List<KeyboardButton>());

            float sumPositionX = ButtonSpacing;
            for (int x = 0; x < keyboardConfiguration.ButtonRows[(keyboardConfiguration.ButtonRows.Count - 1) - y].Buttons.Count; x++)
            {
                GameObject button = Instantiate(ButtonPrefab, transform);
                button.transform.localPosition = new Vector3(sumPositionX - keyboardWidth / 2, ButtonSpacing + (y * ButtonSize + y * ButtonSpacing) - keyboardHeight / 2);

                KeyboardButton keyboardButton = button.GetComponent<KeyboardButton>();
                KeyboardButtonConfiguration buttonConfiguration = keyboardConfiguration.ButtonRows[(keyboardConfiguration.ButtonRows.Count - 1) - y].Buttons[x];
                keyboardButton.Initialize(buttonConfiguration, ButtonSize, FontSize);
                keyboardButton.Button.onClick.AddListener(() => ButtonWasClicked(keyboardButton));

                sumPositionX += buttonConfiguration.GetButtonWidth(ButtonSize, FontSize) + ButtonSpacing;

                buttons[y].Add(keyboardButton);
            }
        }

        for (int y = 0; y < buttons.Count; y++)
        {
            for (int x = 0; x < buttons[y].Count; x++)
            {
                int aboveRow = Mathf.Min(buttons.Count - 1, y + 1);
                int belowRow = Mathf.Max(0, y - 1);
                int right = Mathf.Min(buttons[y].Count - 1, x + 1);
                int left = Mathf.Max(0, x - 1);

                Navigation navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnDown = x >= buttons[belowRow].Count ? buttons[belowRow].Last().Button : buttons[belowRow][x].Button,
                    selectOnUp = x >= buttons[aboveRow].Count ? buttons[aboveRow].Last().Button : buttons[aboveRow][x].Button,
                    selectOnLeft = buttons[y][left].Button,
                    selectOnRight = buttons[y][right].Button,
                };

                buttons[y][x].Button.navigation = navigation;
            }
        }

        buttons[0][0].Button.Select();

        inputActions = new PlayerControls();
        inputActions.Enable();
        inputActions.Ui.EnterText.performed += EnterWasPressed;
    }

    public void SetToPassword()
    {
        TextField.contentType = TMP_InputField.ContentType.Password;
    }

    private void EnterWasPressed(InputAction.CallbackContext obj)
    {
        if (!isEnteringTextWithKeyboard)
        {
            if (WorldEditorUi.Instance != null)
            {
                if (WorldEditorUi.Instance.KeyboardExists != null && (bool)WorldEditorUi.Instance.KeyboardExists)
                {
                    WorldEditorUi.Instance.DisableUiInput();
                    WorldEditor.Instance.DisableControls();
                    TextField.Select();
                    isEnteringTextWithKeyboard = true;
                }
            }
            else if (MainMenuUi.Instance != null)
            {
                MainMenuUi.Instance.DisableUiInput();
                TextField.Select();
                isEnteringTextWithKeyboard = true;
            }
        }
        else
        {
            if (WorldEditorUi.Instance != null)
            {
                WorldEditorUi.Instance.EnableUiInput();
                WorldEditor.Instance.EnableControls();
                inputActions.Ui.EnterText.performed -= EnterWasPressed;
                currentText = new StringBuilder(TextField.text);
                isEnteringTextWithKeyboard = false;
                Submit();
            }
            else if(MainMenuUi.Instance != null)
            {
                MainMenuUi.Instance.EnableUiInput();
                inputActions.Ui.EnterText.performed -= EnterWasPressed;
                currentText = new StringBuilder(TextField.text);
                isEnteringTextWithKeyboard = false;
                Submit();
            }
        }
    }

    public void ButtonWasClicked(KeyboardButton button)
    {
        switch (button.ButtonConfiguration.ButtonType)
        {
            case KeyboardButtonType.Character:
                string text = button.ButtonConfiguration.Text;
                if (text.All(c => c == ' '))
                    text = " ";
                currentText.Append(isShifted ? text.ToUpper() : text);
                isShifted = false;
                break;
            case KeyboardButtonType.Enter:
                Submit();
                break;
            case KeyboardButtonType.Backspace:
                currentText.Remove(currentText.Length - 1, 1);
                break;
            case KeyboardButtonType.Escape:
                Close();
                break;
            case KeyboardButtonType.Shift:
                isShifted = !isShifted;
                break;
            case KeyboardButtonType.CapsLock:
                break;
            default:
                break;
        }

        TextField.text = currentText.ToString();
    }

    private void Submit()
    {
        OnTextSubmitted?.Invoke(this, currentText.ToString());
    }

    private void Close()
    {
        OnClosed?.Invoke(this);
    }
}
