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
            PropertyContainer.GetProperties(behaviour.GetType()).ApplyValues(behaviour);
        }
    }

    public void WasLoaded(BehaviourPropertyContainer propertyContainer)
    {
        PropertyContainer = propertyContainer;
        ApplyValues();
    }
}
