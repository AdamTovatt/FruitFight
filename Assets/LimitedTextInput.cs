using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class LimitedTextInput : MonoBehaviour
{
    private TMP_InputField inputField;

    public int MaxCharacters;
    public TextMeshProUGUI LimitDisplayText;

    private void Awake()
    {
        inputField = gameObject.GetComponent<TMP_InputField>();
        inputField.characterLimit = MaxCharacters;

        UpdateDisplayText(inputField.text);

        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        inputField.onValueChanged.AddListener((newValue) => UpdateDisplayText(newValue));
    }

    private void UnBindEvents()
    {
        inputField.onValueChanged.RemoveAllListeners();
    }

    private void UpdateDisplayText(string newValue)
    {
        LimitDisplayText.text = string.Format("({0}/{1})", newValue.Length, MaxCharacters);
    }
}
