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
    public Color NormalTextColor;

    private bool checkIfPrefab;
    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;

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
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = property.Name;
        PanelDescription.text = stringInputAttribute.Description;
        checkIfPrefab = stringInputAttribute.CheckIfPrefab;
        TextInput.text = (string)property.GetValue(currentBehaviour);
    }

    private void TextWasEntered(string text)
    {
        Debug.Log("text was entered: " + text);
        currentProperty.SetValue(currentBehaviour, TextInput.text);
    }
}
