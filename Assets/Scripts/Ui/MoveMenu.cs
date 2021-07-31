using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveMenu : MonoBehaviour
{
    private const string stringFormat = "0.0";

    public Button SetActivatorButton;
    public Button SetFinalPositionButton;
    public Button CloseButton;
    public Button IncreaseMoveSpeedButton;
    public Button DecreaseMoveSpeedButton;
    public Button IncreaseEndpointDelayButton;
    public Button DecreaseEndpointDelayButton;

    public TextMeshProUGUI CurrentActivatorText;
    public TextMeshProUGUI FinalPositionText;
    public TextMeshProUGUI MoveSpeedText;
    public TextMeshProUGUI EndpointDelayText;

    public Toggle PingPongToggle;
    public Toggle LinearMovementToggle;

    public delegate void OnClosedHandler();
    public event OnClosedHandler OnClosed;

    private Block currentBlock;

    private Color enabledColor;

    private void Awake()
    {
        enabledColor = CurrentActivatorText.color;
    }

    private void Start()
    {
        SetActivatorButton.onClick.AddListener(SetActivator);
        SetFinalPositionButton.onClick.AddListener(SetFinalPosition);
        CloseButton.onClick.AddListener(Close);
        IncreaseMoveSpeedButton.onClick.AddListener(IncreaseMoveSpeed);
        DecreaseMoveSpeedButton.onClick.AddListener(DecreaseMoveSpeed);
        IncreaseEndpointDelayButton.onClick.AddListener(IncreaseEndpointDelay);
        DecreaseEndpointDelayButton.onClick.AddListener(DecreaseEndpointDelay);

        PingPongToggle.onValueChanged.AddListener(PingPongValueChange);
        LinearMovementToggle.onValueChanged.AddListener(LinearMovementValueChange);
    }

    public void ActivatorWasSet(Block activatorBlock)
    {
        SetActivatorButton.Select();

        if (activatorBlock != null)
        {
            currentBlock.BehaviourProperties.MovePropertyCollection.ActivatorBlockId = activatorBlock.Id;
            CurrentActivatorText.text = currentBlock.BehaviourProperties.MovePropertyCollection.ActivatorBlockId.ToString();
            currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
            CurrentActivatorText.color = enabledColor;
            PingPongToggle.isOn = false;
            currentBlock.BehaviourProperties.MovePropertyCollection.PingPong = false;
        }
        else
        {
            WorldEditorUi.Instance.AlertCreator.CreateAlert("That object can not act as an activator").OnOptionWasChosen += (object sender, int optionIndex) => { SetActivatorButton.Select(); };
        }
    }

    private void LinearMovementValueChange(bool newValue)
    {
        currentBlock.BehaviourProperties.MovePropertyCollection.LinearMovement = newValue;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
    }

    private void PingPongValueChange(bool newValue)
    {
        currentBlock.BehaviourProperties.MovePropertyCollection.PingPong = newValue;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;

        if (newValue)
            CurrentActivatorText.color = Color.grey;
        else
            CurrentActivatorText.color = enabledColor;
    }

    private void IncreaseEndpointDelay()
    {
        float endpointDelay = currentBlock.BehaviourProperties.MovePropertyCollection.EndpointDelay;
        endpointDelay = Mathf.Clamp(endpointDelay + 0.1f, 0.1f, 60);
        currentBlock.BehaviourProperties.MovePropertyCollection.EndpointDelay = endpointDelay;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
        EndpointDelayText.text = endpointDelay.ToString(stringFormat);
    }

    private void DecreaseEndpointDelay()
    {
        float endpointDelay = currentBlock.BehaviourProperties.MovePropertyCollection.EndpointDelay;
        endpointDelay = Mathf.Clamp(endpointDelay - 0.1f, 0.1f, 60);
        currentBlock.BehaviourProperties.MovePropertyCollection.EndpointDelay = endpointDelay;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
        EndpointDelayText.text = endpointDelay.ToString(stringFormat);
    }

    private void IncreaseMoveSpeed()
    {
        float moveSpeed = currentBlock.BehaviourProperties.MovePropertyCollection.MoveSpeed;
        moveSpeed = Mathf.Clamp(moveSpeed + 0.1f, 0.1f, 10);
        currentBlock.BehaviourProperties.MovePropertyCollection.MoveSpeed = moveSpeed;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
        MoveSpeedText.text = moveSpeed.ToString(stringFormat);
    }

    private void DecreaseMoveSpeed()
    {
        float moveSpeed = currentBlock.BehaviourProperties.MovePropertyCollection.MoveSpeed;
        moveSpeed = Mathf.Clamp(moveSpeed - 0.1f, 0.1f, 10);
        currentBlock.BehaviourProperties.MovePropertyCollection.MoveSpeed = moveSpeed;
        currentBlock.BehaviourProperties.MovePropertyCollection.HasValues = true;
        MoveSpeedText.text = moveSpeed.ToString(stringFormat);
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

        MoveSpeedText.text = block.BehaviourProperties.MovePropertyCollection.MoveSpeed.ToString(stringFormat);
        EndpointDelayText.text = block.BehaviourProperties.MovePropertyCollection.EndpointDelay.ToString(stringFormat);
        PingPongToggle.isOn = block.BehaviourProperties.MovePropertyCollection.PingPong;
        LinearMovementToggle.isOn = block.BehaviourProperties.MovePropertyCollection.LinearMovement;

        currentBlock = block;
    }
}
