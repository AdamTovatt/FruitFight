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
    public TextMeshProUGUI DisplayTimeText;
    public Image IconImage;
    public TextMeshProUGUI IconText;

    public Toggle ShowMultipleTimesToggle;

    public delegate void OnClosedHandler();
    public event OnClosedHandler OnClosed;

    private Block currentBlock;
    private List<IconConfigurationEntry> icons;
    private int currentIconIndex;

    private void Start()
    {
        icons = IconConfiguration.Get().Icons;
        icons.Add(new IconConfigurationEntry() { Name = "None", FileName = "None" });

        CloseButton.onClick.AddListener(Close);
        SetTextButton.onClick.AddListener(TakeTextInput);
        SetActivatorButton.onClick.AddListener(PickActivator);
        IncreaseIconButton.onClick.AddListener(IncreaseIcon);
        DecreaseIconButton.onClick.AddListener(DecreaseIcon);
        IncreaseDisplayTimeButton.onClick.AddListener(IncreaseDisplayTime);
        DecreaseDisplayTimeButton.onClick.AddListener(DecreaseDisplayTime);
        ShowMultipleTimesToggle.onValueChanged.AddListener(ShowMultipleTimesValueChanged);

        DecreaseIcon();
    }

    private void ShowMultipleTimesValueChanged(bool newValue)
    {
        currentBlock.BehaviourProperties.NotificationPropertyCollection.ShowMultipleTimes = newValue;
        currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;
    }

    private void IncreaseIcon()
    {
        currentIconIndex++;
        if (currentIconIndex >= icons.Count)
            currentIconIndex = 0;

        IconConfigurationEntry icon = icons[currentIconIndex];
        currentBlock.BehaviourProperties.NotificationPropertyCollection.IconName = icon.Name == "None" ? null : icon.Name;
        currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;

        if (icon.Name == "None")
        {
            IconText.enabled = true;
            IconText.text = "None";
            IconImage.enabled = false;
        }
        else
        {
            IconText.enabled = false;
            IconImage.enabled = true;
            IconImage.sprite = AlertCreator.Instance.GetIcon(icon.Name).Image;
        }
    }

    private void DecreaseIcon()
    {
        currentIconIndex--;
        if (currentIconIndex < 0)
            currentIconIndex = icons.Count - 1;

        IconConfigurationEntry icon = icons[currentIconIndex];
        currentBlock.BehaviourProperties.NotificationPropertyCollection.IconName = icon.Name == "None" ? null : icon.Name;
        currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;

        if (icon.Name == "None")
        {
            IconText.enabled = true;
            IconText.text = "None";
            IconImage.enabled = false;
        }
        else
        {
            IconText.enabled = false;
            IconImage.enabled = true;
            IconImage.sprite = AlertCreator.Instance.GetIcon(icon.Name).Image;
        }
    }

    private void IncreaseDisplayTime()
    {
        currentBlock.BehaviourProperties.NotificationPropertyCollection.DisplayTime += 1;
        DisplayTimeText.text = currentBlock.BehaviourProperties.NotificationPropertyCollection.DisplayTime.ToString();
        currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;
    }

    private void DecreaseDisplayTime()
    {
        currentBlock.BehaviourProperties.NotificationPropertyCollection.DisplayTime -= 1;
        DisplayTimeText.text = currentBlock.BehaviourProperties.NotificationPropertyCollection.DisplayTime.ToString();
        currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;
    }

    public void Show(Block block)
    {
        currentBlock = block;

        if (block.BehaviourProperties.NotificationPropertyCollection.HasValues)
        {
            TextText.text = block.BehaviourProperties.NotificationPropertyCollection.Text;
            CurrentActivatorText.text = block.BehaviourProperties.NotificationPropertyCollection.ActivatorBlockId.ToString();

            string iconName = block.BehaviourProperties.NotificationPropertyCollection.IconName;
            if (string.IsNullOrEmpty(iconName) || iconName == "None")
            {
                IconText.enabled = true;
                IconText.text = "None";
                IconImage.enabled = false;
            }
            else
            {
                IconText.enabled = false;
                IconImage.enabled = true;
                IconImage.sprite = AlertCreator.Instance.GetIcon(iconName).Image;
            }
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
        if (success)
        {
            currentBlock.BehaviourProperties.NotificationPropertyCollection.Text = text;
            TextText.text = text;

            currentBlock.BehaviourProperties.NotificationPropertyCollection.HasValues = true;
        }

        WorldEditorUi.Instance.OnScreenKeyboard.OnGotText -= GotTextInput;
        SetTextButton.Select();

        WorldEditor.Instance.DisableControls();
        WorldEditorUi.Instance.EnableUiInput();
    }

    private void TakeTextInput()
    {
        WorldEditorUi.Instance.OnScreenKeyboard.OnGotText += GotTextInput;

        WorldEditorUi.Instance.OnScreenKeyboard.OpenKeyboard();
        Debug.Log("Disable input");
        WorldEditor.Instance.DisableControls();
        WorldEditorUi.Instance.EnableUiInput();
    }

    private void Close()
    {
        OnClosed?.Invoke();
        OnClosed = null;
        currentBlock = null;
    }
}
