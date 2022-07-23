using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObject : BehaviourBase
{
    public class ChildObjectProperties : BehaviourProperties
    {
        [IntInput(Name = "Parent", Description = "The object that is the parent of this object", Limitless = true)]
        public int ParentObjectId { get; set; }

        public ChildObjectProperties() { }

        public ChildObjectProperties(int parentObjectId)
        {
            ParentObjectId = parentObjectId;
            Type = typeof(ChildObjectProperties).Name;
        }

        public override Type BehaviourType => typeof(ChildObject);
    }

    public ChildObjectProperties Properties { get; set; }

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (ChildObjectProperties)behaviourProperties;
        BindEvents();
    }

    private void BindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks += WorldWasBuilt;
    }

    private void UnBindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks -= WorldWasBuilt;
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void WorldWasBuilt()
    {
        Block parent = WorldBuilder.Instance.GetPlacedBlock(Properties.ParentObjectId);

        if (parent.Instance != null)
            gameObject.transform.parent = parent.Instance.transform;
    }
}
