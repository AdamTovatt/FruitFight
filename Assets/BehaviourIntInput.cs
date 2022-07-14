using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourIntInput : MonoBehaviour
{
    public TMP_InputField TextInput;
    public Image TextBackground;
    public TextMeshProUGUI InputDescription;
    public TextMeshProUGUI PanelDescription;
    public Color ErrorColor;
    public Color NormalColor;
    public Color ConfirmedColor;

    public Button AddButton;
    public Button SubtractButton;

    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;
    private IntInputAttribute currentAttribute;

    private void Awake()
    {
        TextInput.onEndEdit.AddListener((text) => TextWasEntered(text));
        AddButton.onClick.AddListener(() => Add());
        SubtractButton.onClick.AddListener(() => Subtract());
    }

    private void OnDestroy()
    {
        TextInput.onEndEdit.RemoveAllListeners();
        AddButton.onClick.RemoveAllListeners();
        SubtractButton.onClick.RemoveAllListeners();
    }

    public void Initialize(IntInputAttribute intInputAttribute, PropertyInfo property, BehaviourProperties behaviour)
    {
        currentAttribute = intInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = intInputAttribute.Name;
        PanelDescription.text = intInputAttribute.Description;

        TextInput.text = ((int)property.GetValue(currentBehaviour)).ToString();

        TextWasEntered(TextInput.text);
    }

    private void TextWasEntered(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        int value = int.Parse(text);

        value = Mathf.Clamp(value, currentAttribute.MinValue, currentAttribute.MaxValue);
        TextInput.text = value.ToString();

        currentProperty.SetValue(currentBehaviour, value);
    }

    private void Add()
    {
        TextWasEntered((int.Parse(TextInput.text) + 1).ToString());
    }

    private void Subtract()
    {
        TextWasEntered((int.Parse(TextInput.text) - 1).ToString());
    }
}
