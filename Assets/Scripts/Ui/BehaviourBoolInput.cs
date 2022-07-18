using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourBoolInput : MonoBehaviour
{
    public Toggle Toggle;
    public TextMeshProUGUI InputDescription;
    public TextMeshProUGUI PanelDescription;

    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;
    private BoolInputAttribute currentAttribute;

    private void Awake()
    {
        Toggle.onValueChanged.AddListener((newValue) => ToggleStateWasChanged(newValue));
    }

    private void OnDestroy()
    {
        Toggle.onValueChanged.RemoveAllListeners();
    }

    public void Initialize(BoolInputAttribute boolInputAttribute, PropertyInfo property, BehaviourProperties behaviour)
    {
        currentAttribute = boolInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = boolInputAttribute.Name;
        PanelDescription.text = boolInputAttribute.Description;

        UpdateToggleGrahpic();
    }

    private void UpdateToggleGrahpic()
    {
        Toggle.isOn = (bool)currentProperty.GetValue(currentBehaviour);
    }

    private void ToggleStateWasChanged(bool newValue)
    {
        currentProperty.SetValue(currentBehaviour, newValue);
    }
}
