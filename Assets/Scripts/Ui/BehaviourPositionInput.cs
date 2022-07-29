using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourPositionInput : AttributeUiInput
{
    public Image ButtonBackground;
    public TextMeshProUGUI InputDescription;
    public TextMeshProUGUI PanelDescription;
    public Color ErrorColor;
    public Color NormalColor;
    public Color ConfirmedColor;

    public Button SelectButton;
    public TextMeshProUGUI ButtonText;

    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;
    private PositionInputAttribute currentAttribute;
    private Block currentBlock;

    BehaviourPropertiesScreen behaviourPropertiesScreen;

    private void Awake()
    {
        SelectButton.onClick.AddListener(() => Select());
    }

    private void OnDestroy()
    {
        SelectButton.onClick.RemoveAllListeners();
    }

    public void Initialize(PositionInputAttribute positionInputAttribute, PropertyInfo property, BehaviourProperties behaviour, Block block, BehaviourPropertiesScreen behaviourPropertiesScreen)
    {
        this.behaviourPropertiesScreen = behaviourPropertiesScreen;

        currentBlock = block;
        currentAttribute = positionInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = positionInputAttribute.Name;
        PanelDescription.text = positionInputAttribute.Description;

        UpdateTextFromProperty();
    }

    private void UpdateTextFromProperty()
    {
        Vector3Int position = (Vector3Int)currentProperty.GetValue(currentBehaviour);
        Debug.Log(position?.GetHashCode());
        string text = "(click to select)";

        if (position != null)
        {
            text = position.ToString();
        }

        ButtonText.text = text;
    }

    private void Select()
    {
        behaviourPropertiesScreen.gameObject.SetActive(false);
        behaviourPropertiesScreen.BackgroundBlur.gameObject.SetActive(false);

        WorldEditor.Instance.OnPositionWasPicked += (Vector3Int position) =>
        {
            currentProperty.SetValue(currentBehaviour, position);
            behaviourPropertiesScreen.gameObject.SetActive(true);
            behaviourPropertiesScreen.BackgroundBlur.gameObject.SetActive(true);
            UpdateTextFromProperty();
        };
        WorldEditor.Instance.PickPosition2(currentBlock);
    }

    public override Selectable GetSelectable()
    {
        return SelectButton;
    }
}
