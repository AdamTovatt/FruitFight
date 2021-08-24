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

        currentBlock.BehaviourProperties.TriggerZonePropertyCollection.IsParent = true;
        currentBlock.BehaviourProperties.TriggerZonePropertyCollection.HasValues = true;
    }
}
