using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventCameraMenu : MonoBehaviour
{
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

    }

    private void SetTarget()
    {

    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
        currentBlock = null;
    }

    private void IncreaseActiveTime()
    {

    }

    private void DecreaseActiveTime()
    {

    }

    private void DeactivateAutomaticallyValueChanged(bool newValue)
    {

    }

    private void ActivateMultipleTimesValueChanged(bool newValue)
    {

    }

    public void Show(Block block)
    {
        currentBlock = block;
        SetActivatorButton.Select();
    }
}
