using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyExposer : MonoBehaviour
{
    public List<MonoBehaviour> Behaviours;
    private BehaviourPropertyContainer PropertyContainer;

    private void ApplyValues()
    {
        foreach(MonoBehaviour behaviour in Behaviours)
        {
            PropertyContainer.GetProperties(behaviour).ApplyValues(behaviour);
        }
    }

    public void WasCreated(Block block)
    {
        foreach(MonoBehaviour behaviour in Behaviours)
        {
            PropertyContainer.Properties.Add(BehaviourPropertyContainer.GetBehaviourProperty(behaviour));
        }

        foreach(BehaviourPropertyCollection behaviourProperties in PropertyContainer.Properties)
        {
            Debug.Log("Has properties: ");
            Debug.Log(behaviourProperties.GetType());
        }
    }

    public void WasLoaded(BehaviourPropertyContainer propertyContainer)
    {
        PropertyContainer = propertyContainer;
    }
}
