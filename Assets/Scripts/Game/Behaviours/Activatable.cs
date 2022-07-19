using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : BehaviourBase
{
    public delegate void ActivatedHandler();
    public event ActivatedHandler OnActivated;

    public delegate void DeActivatedHandler();
    public event DeActivatedHandler OnDeactivated;

    public class ActivatableProperties : BehaviourProperties
    {
        [ActivatorInput(Name = "Activator", Description = "The activator that will activate this object")]
        public int ActivatorObjectId { get; set; }

        [BoolInput(Name = "Listen to activation", Description = "If the object should listen to activation events. This should probably always be activated")]
        public bool ListenToActivation { get; set; } = true;

        [BoolInput(Name = "Listen to deactivation", Description = "If the object should listen to deactivation events. Turn this off if the object should never deactivate once activated")]
        public bool ListenToDeActivation { get; set; } = true;

        public override Type BehaviourType => typeof(Activatable);
    }

    public ActivatableProperties Properties { get; set; }
    private StateSwitcher stateSwitcher;

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (ActivatableProperties)behaviourProperties;

        BindEvents();
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
            stateSwitcher.OnActivated -= StateSwitcherActivated;
            stateSwitcher.OnDeactivated -= StateSwitcherDeactivated;
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
                stateSwitcher.OnActivated += StateSwitcherActivated;
                stateSwitcher.OnDeactivated += StateSwitcherDeactivated;
            }
        }
    }

    private void StateSwitcherActivated()
    {
        if (Properties.ListenToActivation)
            OnActivated?.Invoke();
    }

    private void StateSwitcherDeactivated()
    {
        if (Properties.ListenToDeActivation)
            OnDeactivated?.Invoke();
    }
}
