using Assets.Scripts.Models;
using System;
using UnityEngine;

[Serializable]
public class EventCameraPropertyCollection : BehaviourPropertyCollection
{
    public Vector3 TargetPosition;
    public bool DeactivateAutomatically;
    public float ActiveTime;
    public bool ActivateMultipleTimes;
    public bool HasValues;
    public int ActivatorBlockId;

    public override void ApplyValues(MonoBehaviour behaviour)
    {
        if (behaviour.GetType() != typeof(EventCamera))
            throw new Exception("Only EventCamera can be controlled by this behaviour property keeper");

        if (HasValues)
        {
            EventCamera eventCamera = (EventCamera)behaviour;
            eventCamera.ActiveTime = ActiveTime;
            eventCamera.ActivateMultipleTimes = ActivateMultipleTimes;
            eventCamera.ActivatorBlockId = ActivatorBlockId;
            eventCamera.DeactivateAutomatically = DeactivateAutomatically;
            eventCamera.TargetPosition = TargetPosition;
        }
        else
        {
            Debug.Log("This property collection has no values: " + this.ToString());
        }
    }
}
