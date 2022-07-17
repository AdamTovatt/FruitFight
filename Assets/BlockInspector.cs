using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BlockInspector : MonoBehaviour
{
    public GameObject BehaviourButtonPrefab;
    public TextMeshProUGUI Title;
    public BehaviourPropertiesScreen BehaviourPropertiesScreen;

    private List<BehaviourButton> buttons = new List<BehaviourButton>();
    private Block currentBlock;

    public void Show(Block block)
    {
        BehaviourPropertiesScreen.gameObject.SetActive(false);
        Clean();

        currentBlock = block;
        Title.text = block.Info.Name;

        if (block.Info.AvailableBehaviours != null)
        {
            foreach (string availableBehaviour in block.Info.AvailableBehaviours)
            {
                AddButton(availableBehaviour);
            }
        }
    }

    public void Clean()
    {
        Title.text = "Block Inspector";

        foreach (BehaviourButton button in buttons)
        {
            button.Button.onClick.RemoveAllListeners();
            Destroy(button.gameObject);
        }

        buttons.Clear();
        currentBlock = null;
    }

    public void AddButton(string behaviourName)
    {
        BehaviourButton button = Instantiate(BehaviourButtonPrefab, transform).GetComponent<BehaviourButton>();

        button.Initialize(behaviourName, GetBehaviourProperties(currentBlock, behaviourName, out _) != null);
        button.Button.onClick.AddListener(() => BehaviourClicked(behaviourName));

        buttons.Add(button);
    }

    private void BehaviourClicked(string behaviourName)
    {
        BehaviourProperties properties = GetBehaviourProperties(currentBlock, behaviourName, out Type behaviourType);

        if (properties == null)
        {
            properties = (BehaviourProperties)Activator.CreateInstance(behaviourType);
            properties.Type = behaviourType.Name;
            Debug.Log("Type: " + properties.Type);
            currentBlock.BehaviourProperties2.Add(properties);
        }

        BehaviourPropertiesScreen.gameObject.SetActive(true);
        BehaviourPropertiesScreen.Show(currentBlock, properties);
    }

    private BehaviourProperties GetBehaviourProperties(Block block, string behaviourName, out Type behaviourType)
    {
        behaviourType = BehaviourProperties.GetTypeFromName(behaviourName);
        Type type = behaviourType; //we need to do this or we can't use it in the linq expression later apparently

        if (behaviourType == null)
            throw new Exception("No type found: " + behaviourName);

        return currentBlock.BehaviourProperties2.Where(x => x.GetType() == type).FirstOrDefault();
    }
}
