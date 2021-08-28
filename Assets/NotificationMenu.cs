using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationMenu : MonoBehaviour
{
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

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        SetTextButton.onClick.AddListener(TakeTextInput);
        SetActivatorButton.onClick.AddListener(PickActivator);
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

    private void PickActivator()
    {
        WorldEditor.Instance.PickActivator(this, currentBlock);
    }

    public void ActivatorWasSet(Block activatorBlock)
    {
        currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;
        currentBlock.BehaviourProperties.NotificationPropertyCollection.ActivatorBlockId = activatorBlock.Id;
        CurrentActivatorText.text = activatorBlock.Id.ToString();
    }

    public void GotTextInput(object sender, bool success, string text)
    {
        if(success)
        {
            currentBlock.BehaviourProperties.NotificationPropertyCollection.Text = text;
            TextText.text = text;

            currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;
        }

        WorldEditorUi.Instance.OnScreenKeyboard.OnGotText -= GotTextInput;
        SetTextButton.Select();
    }

    private void TakeTextInput()
    {
        WorldEditorUi.Instance.OnScreenKeyboard.OnGotText += GotTextInput;

        WorldEditorUi.Instance.OnScreenKeyboard.OpenKeyboard();
    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
        currentBlock = null;
    }
}
