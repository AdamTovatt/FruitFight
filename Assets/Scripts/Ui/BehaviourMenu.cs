using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourMenu : MonoBehaviour
{
    public Button MoveButton;
    public Button DetailColorButton;
    public Button TriggerZoneButton;
    public Button NotificationButton;
    public Button CloseButton;
    public TextMeshProUGUI DetailColorButtonText;
    public TextMeshProUGUI TriggerZoneButtonText;
    public TextMeshProUGUI NotificationButtonText;

    public DetailColorMenu DetailColorMenu;
    public MoveMenu MoveMenu;
    public EventCameraMenu EventCameraMenu;
    
    public GameObject BehaviourPanel;

    private Block currentBlock;
    private DetailColorController currentDetailColor;
    private TriggerZone currentTriggerZone;
    private NotificationBlock currentNotificationBlock;
    private EventCamera currentEventCamera;

    private Color enabledColor;

    private void Awake()
    {
        enabledColor = DetailColorButtonText.color;
    }

    private void Start()
    {
        MoveButton.onClick.AddListener(() => { Move(); });
        DetailColorButton.onClick.AddListener(() => { DetailColor(); });
        CloseButton.onClick.AddListener(() => { WorldEditorUi.Instance.CloseBehaviourMenu(); });
    }

    private void DetailColor()
    {
        currentDetailColor = currentBlock.Instance.GetComponent<DetailColorController>();
        if (currentDetailColor == null)
            return;

        DetailColorMenu.gameObject.SetActive(true);
        DetailColorMenu.NextColorButton.Select();
        DetailColorMenu.Show(currentBlock);
        DetailColorMenu.OnClosed += (DetailColor color) => 
        {
            DetailColorPropertyCollection detailColorCollection = ((DetailColorPropertyCollection)currentBlock.BehaviourProperties.GetProperties(typeof(DetailColorController)));
            detailColorCollection.Color = color;
            detailColorCollection.ApplyValues(currentDetailColor);
            DetailColorMenu.gameObject.SetActive(false);
            CloseButton.Select();
        };
    }

    private void Move()
    {
        if (!currentBlock.HasPropertyExposer)
            currentBlock.BehaviourProperties = new BehaviourPropertyContainer();

        currentBlock.HasPropertyExposer = true;

        if (currentBlock.BehaviourProperties.MovePropertyCollection == null || !currentBlock.BehaviourProperties.MovePropertyCollection.HasValues)
            currentBlock.BehaviourProperties.MovePropertyCollection = new MovePropertyCollection();

        MoveMenu.gameObject.SetActive(true);
        MoveMenu.Show(currentBlock);

        MoveMenu.OnClosed += () =>
        {
            MoveMenu.gameObject.SetActive(false);
            CloseButton.Select();
        };
    }

    public void EventCamera()
    {
        BehaviourPanel.gameObject.SetActive(false);

        EventCameraMenu.gameObject.SetActive(true);
        EventCameraMenu.Show(currentBlock);

        EventCameraMenu.OnClosed += () =>
        {
            WorldEditorUi.Instance.CloseBehaviourMenu();
            EventCameraMenu.gameObject.SetActive(false);
            BehaviourPanel.gameObject.SetActive(true);
        };
    }

    public void Hide()
    {
        MoveMenu.gameObject.SetActive(false);
        DetailColorMenu.gameObject.SetActive(false);
        EventCameraMenu.gameObject.SetActive(false);
    }

    public void Show(Block block)
    {
        if (block.Instance == null)
            return;

        currentBlock = block;
        currentDetailColor = block.Instance.GetComponent<DetailColorController>();
        currentTriggerZone = block.Instance.GetComponent<TriggerZone>();
        currentNotificationBlock = block.Instance.GetComponent<NotificationBlock>();
        currentEventCamera = block.Instance.GetComponent<EventCamera>();

        if(currentEventCamera != null) //if this is an event camera we will show the event camera menu straight away
        {
            EventCamera();
            return;
        }

        if (currentDetailColor == null)
            DetailColorButtonText.color = Color.grey;
        else
            DetailColorButtonText.color = enabledColor;

        if (currentNotificationBlock == null)
            NotificationButtonText.color = Color.grey;
        else
            NotificationButtonText.color = enabledColor;

        MoveButton.Select();
    }
}
