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

    public void ActivatorWasSet(Block activatorBlock)
    {
        SetActivatorButton.Select();

        if (activatorBlock != null)
        {
            currentBlock.BehaviourProperties.MovePropertyCollection.ActivatorBlockId = activatorBlock.Id;
            CurrentActivatorText.text = currentBlock.BehaviourProperties.MovePropertyCollection.ActivatorBlockId.ToString();
            currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
        }
        else
        {
            WorldEditorUi.Instance.AlertCreator.CreateAlert("That object can not act as an activator").OnOptionWasChosen += (object sender, int optionIndex) => { SetActivatorButton.Select(); };
        }
    }

    private void SetActivator()
    {
        WorldEditor.Instance.PickActivator(this);
    }

    public void FinalPositionWasSet(Vector3Int position)
    {
        currentBlock.BehaviourProperties.MovePropertyCollection.FinalPosition = position;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
        FinalPositionText.text = position.ToString();
        SetActivatorButton.Select();
    }

    private void SetFinalPosition()
    {
        WorldEditor.Instance.PickMoveFinalPosition(this, currentBlock);
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
