using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public TextMeshProUGUI Text;
    public int ButtonHeight = 50;

    public delegate void OptionWasChosenHandler(object sender, int buttonIndex);
    public event OptionWasChosenHandler OnOptionWasChosen;

    public void Initialize(string text, List<string> buttons)
    {
        Text.text = text;

        List<UiButton> createdButtons = new List<UiButton>();

        if (buttons != null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                UiButton button = Instantiate(ButtonPrefab, transform).GetComponent<UiButton>();
                button.transform.position = new Vector3(transform.position.x, transform.position.y - ButtonHeight * (i + 1) - 20, transform.position.z);
                createdButtons.Add(button);
                int index = i;
                button.ButtonComponent.onClick.AddListener(() => { ButtonWasClicked(index); });
            }

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + (buttons.Count) * ButtonHeight + (ButtonHeight * 1.4f));
        }

        for (int i = 0; i < createdButtons.Count; i++)
        {
            UiButton up = i == 0 ? createdButtons[createdButtons.Count - 1] : createdButtons[i - 1];
            UiButton down = i == createdButtons.Count - 1 ? createdButtons[0] : createdButtons[i + 1];
            createdButtons[i].Initialize(buttons[i], up, down);
        }

        if(buttons != null)
        {
            createdButtons[0].ButtonComponent.Select();
        }
    }

    private void ButtonWasClicked(int index)
    {
        OnOptionWasChosen?.Invoke(this, index);
        Destroy(gameObject);
    }
}
