using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public TextMeshProUGUI Text;

    public delegate void OptionWasChosenHandler(object sender, int buttonIndex);
    public event OptionWasChosenHandler OnOptionWasChosen;

    public void Initialize(string text, List<string> buttons)
    {
        Text.text = text;

        List<UiButton> createdButtons = new List<UiButton>();

        float sizeMultiplier = gameObject.GetComponentInParent<Canvas>().transform.localScale.magnitude;

        float buttonHeight = ButtonPrefab.GetComponent<RectTransform>().rect.height;
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                UiButton button = Instantiate(ButtonPrefab, transform).GetComponent<UiButton>();
                button.transform.position = new Vector3(transform.position.x, transform.position.y - (buttonHeight * sizeMultiplier) * (i + 1) - (20 * sizeMultiplier), transform.position.z);
                createdButtons.Add(button);
                int index = i;
                button.ButtonComponent.onClick.AddListener(() => { ButtonWasClicked(index); });
            }

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + (buttons.Count) * buttonHeight + (buttonHeight * 1.4f));
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
        OnOptionWasChosen = null;
        Destroy(gameObject);
    }
}
