using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriggerZoneMenu : MonoBehaviour
{
    public Button AddSubZoneButton;
    public Button CloseButton;

    public TextMeshProUGUI CurrentSubZonesText;

    public delegate void OnClosedHandler();
    public event OnClosedHandler OnClosed;

    private Block currentBlock;

    private void Start()
    {
        AddSubZoneButton.onClick.AddListener(AddSubZone);
        CloseButton.onClick.AddListener(Close);
    }

    private void AddSubZone()
    {
        WorldEditor.Instance.AddTriggerSubZone(this, currentBlock);
    }

    public void SubZoneWasAdded(Block subZone)
    {
        WorldEditorUi.Instance.AlertCreator.CreateNotification("Sub zone was added to trigger", 2);
        currentBlock.BehaviourProperties.TriggerZonePropertyCollection.Children.Add(subZone.Id);

        CurrentSubZonesText.text = currentBlock.BehaviourProperties.TriggerZonePropertyCollection.Children.Count.ToString();
    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
    }

    public void Show(Block block)
    {
        currentBlock = block;
        
        if(currentBlock.BehaviourProperties == null)
            currentBlock.BehaviourProperties = new BehaviourPropertyContainer();

        if (currentBlock.BehaviourProperties.TriggerZonePropertyCollection == null)
            currentBlock.BehaviourProperties.TriggerZonePropertyCollection = new TriggerZonePropertyCollection();

        CurrentSubZonesText.text = currentBlock.BehaviourProperties.TriggerZonePropertyCollection.Children.Count.ToString();

        currentBlock.BehaviourProperties.TriggerZonePropertyCollection.IsParent = true;
        currentBlock.BehaviourProperties.TriggerZonePropertyCollection.HasValues = true;
    }
}
