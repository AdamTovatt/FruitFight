using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourFloatInput : AttributeUiInput
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
    private FloatInputAttribute currentAttribute;

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

    public void Initialize(FloatInputAttribute floatInputAttribute, PropertyInfo property, BehaviourProperties behaviour)
    {
        currentAttribute = floatInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = floatInputAttribute.Name;
        PanelDescription.text = floatInputAttribute.Description;

        TextInput.text = ((float)property.GetValue(currentBehaviour)).ToString();

        TextWasEntered(TextInput.text);
    }

    private void TextWasEntered(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        float value = float.Parse(text);

        value = Mathf.Clamp(value, currentAttribute.MinValue, currentAttribute.MaxValue);
        TextInput.text = value.ToString();

        currentProperty.SetValue(currentBehaviour, value);
    }

    private void Add()
    {
        TextWasEntered((float.Parse(TextInput.text) + 0.1f).ToString());
    }

    private void Subtract()
    {
        TextWasEntered((float.Parse(TextInput.text) - 0.1f).ToString());
    }

    public override Selectable GetSelectable()
    {
        return SubtractButton;
    }
}
