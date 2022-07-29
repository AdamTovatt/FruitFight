using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourActivatorInput : AttributeUiInput
{
    public Image ButtonBackground;
    public TextMeshProUGUI InputDescription;
    public TextMeshProUGUI PanelDescription;
    public Color ErrorColor;
    public Color NormalColor;
    public Color ConfirmedColor;

    public Button SelectButton;
    public TextMeshProUGUI ButtonText;
    public Button RemoveButton;

    private PropertyInfo currentProperty;
    private BehaviourProperties currentBehaviour;
    private ActivatorInputAttribute currentAttribute;
    private Block currentBlock;

    BehaviourPropertiesScreen behaviourPropertiesScreen;

    private void Awake()
    {
        SelectButton.onClick.AddListener(() => Select());
        RemoveButton.onClick.AddListener(() => Remove());
    }

    private void OnDestroy()
    {
        SelectButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.RemoveAllListeners();
    }

    public void Initialize(ActivatorInputAttribute activatorInputAttribute, PropertyInfo property, BehaviourProperties behaviour, Block block, BehaviourPropertiesScreen behaviourPropertiesScreen)
    {
        this.behaviourPropertiesScreen = behaviourPropertiesScreen;

        currentBlock = block;
        currentAttribute = activatorInputAttribute;
        currentProperty = property;
        currentBehaviour = behaviour;
        InputDescription.text = activatorInputAttribute.Name;
        PanelDescription.text = activatorInputAttribute.Description;

        UpdateTextFromProperty();
    }

    private void UpdateTextFromProperty()
    {
        int blockId = ((int)currentProperty.GetValue(currentBehaviour));
        Block block = WorldEditor.Instance.CurrentWorld.Blocks.Where(x => x.Id == blockId).FirstOrDefault();
        string text = "(click to select)";

        if (block != null)
        {
            text = string.Format("{0} ({1})", block.Info.Name, blockId);
        }

        ButtonText.text = text;
    }

    private void Remove()
    {
        currentProperty.SetValue(currentBehaviour, 0);
        UpdateTextFromProperty();
    }

    private void Select()
    {
        if (WorldEditor.Instance.GetCurrentlyAvailableActivators().Count == 0)
        {
            AlertCreator.Instance.CreateNotification("No activators exist in this world, place one first");
            return;
        }

        behaviourPropertiesScreen.gameObject.SetActive(false);
        behaviourPropertiesScreen.BackgroundBlur.gameObject.SetActive(false);

        WorldEditor.Instance.OnActivatorWasPicked += (Block block) =>
        {
            if (block != null)
            {
                currentProperty.SetValue(currentBehaviour, block.Id);
            }

            behaviourPropertiesScreen.gameObject.SetActive(true);
            behaviourPropertiesScreen.BackgroundBlur.gameObject.SetActive(true);
            UpdateTextFromProperty();

        };
        WorldEditor.Instance.PickActivator2(currentBlock);
    }

    public override Selectable GetSelectable()
    {
        return SelectButton;
    }
}
