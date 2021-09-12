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
    public TriggerZoneMenu TriggerZoneMenu;
    public NotificationMenu NotificationMenu;

    private Block currentBlock;
    private DetailColorController currentDetailColor;
    private TriggerZone currentTriggerZone;
    private NotificationBlock currentNotificationBlock;

    private Color enabledColor;

    private void Awake()
    {
        enabledColor = DetailColorButtonText.color;
    }

    private void Start()
    {
        MoveButton.onClick.AddListener(() => { Move(); });
        DetailColorButton.onClick.AddListener(() => { DetailColor(); });
        TriggerZoneButton.onClick.AddListener(TriggerZone);
        CloseButton.onClick.AddListener(() => { WorldEditorUi.Instance.CloseBehaviourMenu(); });
        NotificationButton.onClick.AddListener(Notification);
    }

    private void TriggerZone()
    {
        currentTriggerZone = currentBlock.Instance.GetComponent<TriggerZone>();
        if (currentTriggerZone == null || !currentTriggerZone.IsParent)
            return;

        TriggerZoneMenu.gameObject.SetActive(true);
        TriggerZoneMenu.AddSubZoneButton.Select();
        TriggerZoneMenu.Show(currentBlock);

        TriggerZoneMenu.OnClosed += () =>
        {
            TriggerZoneMenu.gameObject.SetActive(false);
            CloseButton.Select();
        };
    }

    private void Notification()
    {
        NotificationMenu.gameObject.SetActive(true);
        NotificationMenu.SetActivatorButton.Select();
        NotificationMenu.Show(currentBlock);

        NotificationMenu.OnClosed += () =>
        {
            NotificationMenu.gameObject.SetActive(false);
            CloseButton.Select();
        };
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

    public void Hide()
    {
        MoveMenu.gameObject.SetActive(false);
        DetailColorMenu.gameObject.SetActive(false);
        NotificationMenu.gameObject.SetActive(false);
        TriggerZoneMenu.gameObject.SetActive(false);
    }

    public void Show(Block block)
    {
        if (block.Instance == null)
            return;

        currentBlock = block;
        currentDetailColor = block.Instance.GetComponent<DetailColorController>();
        currentTriggerZone = block.Instance.GetComponent<TriggerZone>();
        currentNotificationBlock = block.Instance.GetComponent<NotificationBlock>();

        if (currentDetailColor == null)
            DetailColorButtonText.color = Color.grey;
        else
            DetailColorButtonText.color = enabledColor;

        if (currentTriggerZone == null || !currentTriggerZone.IsParent)
            TriggerZoneButtonText.color = Color.grey;
        else
            TriggerZoneButtonText.color = enabledColor;

        if (currentNotificationBlock == null)
            NotificationButtonText.color = Color.grey;
        else
            NotificationButtonText.color = enabledColor;

        MoveButton.Select();
    }
}
