using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationBlock : BehaviourBase
{
    public class NotificationBlockProperties : BehaviourProperties
    {
        [StringInput(Description = "The name of the icon to display, if any", Name = "Icon")]
        public string IconName { get; set; }

        [StringInput(Name = "Text", Description = "The text to show in the notification")]
        public string Text { get; set; }

        [IntInput(Name = "Display time", Description = "The time the notification will be displayed for in seconds", MinValue = 0, MaxValue = 10)]
        public int DisplayTime { get; set; }

        [BoolInput(Name = "Show multiple times", Description = "If the notification should be shown multiple times or not")]
        public bool ShowMultipleTimes { get; set; }

        [ActivatorInput(Description = "The activator which will activate this movement", Name = "Activator")]
        public int ActivatorObjectId { get; set; }

        public override Type BehaviourType => typeof(NotificationBlock);
    }

    [JsonIgnore]
    public AlwaysFaceCamera IconRotator;
    [JsonIgnore]
    public MeshRenderer IconGraphic;

    public NotificationBlockProperties Properties { get; set; }

    private bool hasBeenActivated = false;
    private StateSwitcher stateSwitcher;

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (NotificationBlockProperties)behaviourProperties;

        BindEvents();
    }

    public void ShowNotification()
    {
        if (!hasBeenActivated || Properties.ShowMultipleTimes)
        {
            hasBeenActivated = true;
            AlertCreator.Instance.CreateNotification(Properties.Text, Properties.DisplayTime, Properties.IconName);
        }
    }

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            if (IconRotator != null)
                IconRotator.DeActivate();
        }
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks += WorldWasBuilt;
    }

    private void UnBindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks -= WorldWasBuilt;

        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated -= ShowNotification;
        }
    }

    private void WorldWasBuilt()
    {
        Block activator = WorldBuilder.Instance.GetPlacedBlock(Properties.ActivatorObjectId);

        if (activator != null && activator.Instance != null)
        {
            stateSwitcher = activator.Instance.GetComponent<StateSwitcher>();

            if (stateSwitcher != null)
            {
                stateSwitcher.OnActivated += ShowNotification;
            }
        }
    }
}
