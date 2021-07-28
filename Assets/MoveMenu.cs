using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveMenu : MonoBehaviour
{
    public Button SetActivatorButton;
    public Button SetFinalPositionButton;
    public Button CloseButton;

    public TextMeshProUGUI CurrentActivatorText;
    public TextMeshProUGUI FinalPositionText;

    public delegate void OnClosedHandler();
    public event OnClosedHandler OnClosed;

    private Block currentBlock;

    private void Start()
    {
        SetActivatorButton.onClick.AddListener(SetActivator);
        SetFinalPositionButton.onClick.AddListener(SetFinalPosition);
        CloseButton.onClick.AddListener(Close);
    }

    private void SetActivator()
    {
        Debug.Log("set activator");
    }

    private void SetFinalPosition()
    {
        Debug.Log("set final position");
    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
    }

    public void Show(Block block)
    {
        SetActivatorButton.Select();

        if(block.BehaviourProperties.MovePropertyCollection.HasValues)
        {
            CurrentActivatorText.text = block.BehaviourProperties.MovePropertyCollection.ActivatorBlockId.ToString();
            FinalPositionText.text = block.BehaviourProperties.MovePropertyCollection.FinalPosition.ToString();
        }
        else
        {
            CurrentActivatorText.text = "not set";
            FinalPositionText.text = "not set";
        }

        currentBlock = block;
    }
}
