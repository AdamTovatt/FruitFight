using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NotificationPropertyCollection : BehaviourPropertyCollection
{
    public string Text;
    public string IconName;
    public int ActivatorBlockId;
    public int DisplayTime = 1;
    public bool ShowMultipleTimes;
    public bool HasValues;

    public override void ApplyValues(MonoBehaviour behaviour)
    {
        if (behaviour.GetType() != typeof(NotificationBlock))
            throw new Exception("Only NotificationBlock can be controlled by this behaviour property keeper");

        if (HasValues)
        {
            NotificationBlock notificationBlock = (NotificationBlock)behaviour;
            notificationBlock.Text = Text;
            notificationBlock.DisplayTime = DisplayTime;
            notificationBlock.ActivatorBlockId = ActivatorBlockId;
            notificationBlock.IconName = IconName;
            notificationBlock.ShowMultipleTimes = ShowMultipleTimes;

        }
        else
        {
            Debug.Log("This property collection has no values: " + this.ToString());
        }
    }
}
