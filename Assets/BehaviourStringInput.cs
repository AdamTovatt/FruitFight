using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourStringInput : MonoBehaviour
{
    public TMP_InputField TextInput;
    public Image TextBackground;
    public TextMeshProUGUI InputDescription;
    public TextMeshProUGUI PanelDescription;
    public Color ErrorColor;
    public Color NormalColor;
    public Color ConfirmedColor;

    private bool checkIfPrefab;
    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;
    private StringInputAttribute currentAttribute;

    private void Awake()
    {
        TextInput.onEndEdit.AddListener((text) => TextWasEntered(text));
    }

    private void OnDestroy()
    {
        TextInput.onEndEdit.RemoveAllListeners();
    }

    public void Initialize(StringInputAttribute stringInputAttribute, PropertyInfo property, BehaviourProperties behaviour)
    {
        currentAttribute = stringInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = stringInputAttribute.Name;
        PanelDescription.text = stringInputAttribute.Description;
        checkIfPrefab = stringInputAttribute.CheckIfPrefab;
        TextInput.text = (string)property.GetValue(currentBehaviour);

        TextWasEntered(TextInput.text);
    }

    private void TextWasEntered(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (checkIfPrefab)
        {
            if (!PrefabLookup.PrefabExists(text))
                SetErrorState();
            else
                SetConfirmedState();
        }

        currentProperty.SetValue(currentBehaviour, TextInput.text);
    }

    private void SetConfirmedState()
    {
        PanelDescription.text = currentAttribute.Description;
        TextBackground.color = ConfirmedColor;
    }

    private void SetErrorState()
    {
        PanelDescription.text = "That prefab does not exist!";
        TextBackground.color = ErrorColor;
    }

    private void SetNormalState()
    {
        PanelDescription.text = currentAttribute.Description;
        TextBackground.color = NormalColor;
    }
}
