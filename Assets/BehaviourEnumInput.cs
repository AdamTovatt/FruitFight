using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourEnumInput : MonoBehaviour
{
    public TMP_InputField TextInput;
    public Image TextBackground;
    public TextMeshProUGUI InputDescription;
    public TextMeshProUGUI PanelDescription;
    public Color ErrorColor;
    public Color NormalColor;
    public Color ConfirmedColor;

    public Button NextButton;
    public Button PreviousButton;

    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;
    private EnumInputAttribute currentAttribute;
    private Array enumValues;
    private int currentEnumIndex;

    private void Awake()
    {
        NextButton.onClick.AddListener(() => Next());
        PreviousButton.onClick.AddListener(() => Previous());
    }

    private void OnDestroy()
    {
        NextButton.onClick.RemoveAllListeners();
        PreviousButton.onClick.RemoveAllListeners();
    }

    public void Initialize(EnumInputAttribute enumInputAttribute, PropertyInfo property, BehaviourProperties behaviour)
    {
        currentAttribute = enumInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = enumInputAttribute.Name;
        PanelDescription.text = enumInputAttribute.Description;

        enumValues = Enum.GetValues(enumInputAttribute.EnumType);
        currentEnumIndex = FindIndex((Enum)currentProperty.GetValue(currentBehaviour));

        UpdateTextFromProperty();
    }

    private void UpdateTextFromProperty()
    {
        TextInput.text = ((Enum)currentProperty.GetValue(currentBehaviour)).ToString();
    }

    private void Next()
    {
        currentEnumIndex++;
        currentEnumIndex = currentEnumIndex % enumValues.Length;
        currentProperty.SetValue(currentBehaviour, enumValues.GetValue(currentEnumIndex));
        UpdateTextFromProperty();
    }

    private void Previous()
    {
        currentEnumIndex--;
        currentEnumIndex = currentEnumIndex % enumValues.Length;
        if (currentEnumIndex < 0) currentEnumIndex = enumValues.Length - 1;
        currentProperty.SetValue(currentBehaviour, enumValues.GetValue(currentEnumIndex));
        UpdateTextFromProperty();
    }

    private int FindIndex(Enum value)
    {
        for (int i = 0; i < enumValues.Length; i++)
        {
            if (enumValues.GetValue(i) == value)
                return i;
        }

        return 0;
    }
}
