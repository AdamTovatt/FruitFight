using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateComponentOnLevelStart : MonoBehaviour
{
    public List<Behaviour> Components;

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            foreach (Behaviour component in Components)
            {
                component.enabled = true;
            }
        }
    }
}
