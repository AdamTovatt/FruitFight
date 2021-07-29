using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyExposer : MonoBehaviour
{
    public List<MonoBehaviour> Behaviours = new List<MonoBehaviour>();
    private BehaviourPropertyContainer PropertyContainer;

    private void ApplyValues()
    {
        if (Behaviours != null)
        {
            foreach (MonoBehaviour behaviour in Behaviours)
            {
                BehaviourPropertyCollection behaviourPropertyCollection = PropertyContainer.GetProperties(behaviour.GetType());

                if (behaviourPropertyCollection != null)
                    behaviourPropertyCollection.ApplyValues(behaviour);
            }
        }
    }

    public void WasLoaded(BehaviourPropertyContainer propertyContainer)
    {
        PropertyContainer = propertyContainer;
        ApplyValues();
    }
}
