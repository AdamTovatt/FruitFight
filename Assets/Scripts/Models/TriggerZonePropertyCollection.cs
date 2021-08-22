using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerZonePropertyCollection : BehaviourPropertyCollection
{
    public bool IsParent;
    public int ParentId;
    public bool HasValues;

    public override void ApplyValues(MonoBehaviour behaviour)
    {
        if (behaviour.GetType() != typeof(TriggerZone))
            throw new Exception("Only TriggerZone can be controlled by this behaviour property keeper");

        if (HasValues)
        {
            TriggerZone triggerZone = (TriggerZone)behaviour;
            triggerZone.IsParent = IsParent;
        }
        else
        {
            Debug.LogError("This property collection has no values: " + this.ToString());
        }
    }
}
