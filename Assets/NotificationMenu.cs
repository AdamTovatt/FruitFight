using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationMenu : MonoBehaviour
{
    private const string stringFormat = "0.0";

    public Button SetActivatorButton;
    public Button SetTextButton;
    public Button IncreaseIconButton;
    public Button DecreaseIconButton;
    public Button IncreaseDisplayTimeButton;
    public Button DecreaseDisplayTimeButton;
    public Button CloseButton;

    public TextMeshProUGUI CurrentActivatorText;
    public TextMeshProUGUI TextText;
    public TextMeshProUGUI IconText;
    public TextMeshProUGUI DisplayTimeText;

    public Toggle ShowMultipleTimesToggle;

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
        CloseButton.onClick.AddListener(Close);
    }

    public void Show(Block block)
    {
        currentBlock = block;

        if(block.BehaviourProperties.NotificationPropertyCollection.HasValues)
        {
            TextText.text = block.BehaviourProperties.NotificationPropertyCollection.Text;
            CurrentActivatorText.text = block.BehaviourProperties.NotificationPropertyCollection.ActivatorBlockId.ToString();
            IconText.text = block.BehaviourProperties.NotificationPropertyCollection.IconName;
        }
        else
        {
            TextText.text = "not set";
            CurrentActivatorText.text = "not set";
        }

        ShowMultipleTimesToggle.isOn = block.BehaviourProperties.NotificationPropertyCollection.ShowMultipleTimes;
        DisplayTimeText.text = block.BehaviourProperties.NotificationPropertyCollection.DisplayTime.ToString();
    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
        currentBlock = null;
    }
}
