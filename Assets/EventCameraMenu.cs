using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventCameraMenu : MonoBehaviour
{
    private const string stringFormat = "0.0";

    public Button SetActivatorButton;
    public Button SetTargetButton;
    public Button CloseButton;
    public Button IncreaseActiveTimeButton;
    public Button DecreaseActiveTimeButton;

    public TextMeshProUGUI CurrentActivatorText;
    public TextMeshProUGUI CurrentTargetPositionText;
    public TextMeshProUGUI ActiveTimeText;

    public Toggle DeactivateAutomaticallyToggle;
    public Toggle ActivateMultipleTimesToggle;

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
        SetTargetButton.onClick.AddListener(SetTarget);
        CloseButton.onClick.AddListener(Close);
        IncreaseActiveTimeButton.onClick.AddListener(IncreaseActiveTime);
        DecreaseActiveTimeButton.onClick.AddListener(DecreaseActiveTime);
        DeactivateAutomaticallyToggle.onValueChanged.AddListener(DeactivateAutomaticallyValueChanged);
        ActivateMultipleTimesToggle.onValueChanged.AddListener(ActivateMultipleTimesValueChanged);
    }

    private void Update()
    {

    }

    private void SetActivator()
    {
        WorldEditor.Instance.PickActivator(this, currentBlock);
    }

    private void SetTarget()
    {
        WorldEditor.Instance.PickPosition(this, currentBlock);
    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
        currentBlock = null;
    }

    private void IncreaseActiveTime()
    {
        float activeTime = currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime;
        activeTime = Mathf.Clamp(activeTime + 0.1f, 0.1f, 10f);
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime = activeTime;
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.DeactivateAutomatically = true;
        ActiveTimeText.text = currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime.ToString(stringFormat);
    }

    private void DecreaseActiveTime()
    {
        float activeTime = currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime;
        activeTime = Mathf.Clamp(activeTime - 0.1f, 0.1f, 10f);
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime = activeTime;
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.DeactivateAutomatically = true;
        ActiveTimeText.text = currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime.ToString(stringFormat);
    }

    private void DeactivateAutomaticallyValueChanged(bool newValue)
    {
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.DeactivateAutomatically = newValue;
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.HasValues = true;
    }

    private void ActivateMultipleTimesValueChanged(bool newValue)
    {
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActivateMultipleTimes = newValue;
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.HasValues = true;
    }

    public void PositionWasPicked(Vector3Int position)
    {
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.TargetPosition = position;
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.HasValues = true;
        CurrentTargetPositionText.text = position.ToString();
        SetActivatorButton.Select();
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.ApplyValues(currentBlock.Instance.GetComponent<EventCamera>());
    }

    public void ActivatorWasSet(Block activatorBlock)
    {
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.HasValues = true;
        currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActivatorBlockId = activatorBlock.Id;
        CurrentActivatorText.text = activatorBlock.Id.ToString();
    }

    public void Show(Block block)
    {
        currentBlock = block;
        SetActivatorButton.Select();

        if (block.BehaviourProperties.EventCameraPropertyCollection.HasValues)
        {
            CurrentActivatorText.text = block.BehaviourProperties.EventCameraPropertyCollection.ActivatorBlockId.ToString();
            CurrentTargetPositionText.text = block.BehaviourProperties.EventCameraPropertyCollection.TargetPosition.ToString();
        }
        else
        {
            CurrentActivatorText.text = "not set";
            CurrentTargetPositionText.text = "not set";
        }

        ActiveTimeText.text = currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActiveTime.ToString(stringFormat);
        ActivateMultipleTimesToggle.isOn = currentBlock.BehaviourProperties.EventCameraPropertyCollection.ActivateMultipleTimes;
        DeactivateAutomaticallyToggle.isOn = currentBlock.BehaviourProperties.EventCameraPropertyCollection.DeactivateAutomatically;
    }
}
