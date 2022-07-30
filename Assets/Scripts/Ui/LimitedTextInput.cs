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
    public bool OnlyShowLimitIfAlmostReached;

    private void Awake()
    {
        inputField = gameObject.GetComponent<TMP_InputField>();
        inputField.characterLimit = MaxCharacters;

        UpdateDisplayText(inputField.text);

        BindEvents();

        if (OnlyShowLimitIfAlmostReached)
            LimitDisplayText.gameObject.SetActive(false);
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
        if (OnlyShowLimitIfAlmostReached && newValue.Length < MaxCharacters * 0.8f)
        {
            LimitDisplayText.gameObject.SetActive(false);
        }
        else
        {
            LimitDisplayText.gameObject.SetActive(true);
            LimitDisplayText.text = string.Format("({0}/{1})", newValue.Length, MaxCharacters);
        }
    }
}
