using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : BehaviourBase
{
    public class TriggerZoneProperties : BehaviourProperties
    {
        [BoolInput(Name = "Parent", Description = "If this zone is a parent zone or not. Should not be changed manually.")]
        public bool IsParent { get; set; } = true;
        
        [IntInput(Name = "Parent Id", Description = "Id of the parent block. Should not be changed manually.", Limitless = true)]
        public int ParentId { get; set; }

        [JsonIgnore]
        [SubZoneInput(Name = "Add sub zone", Description = "Add a sub zone to this trigger zone. The sub zone will act as an extension of this zone")]
        public int AddTriggerSubZone { get; set; }

        public override Type BehaviourType => typeof(TriggerZone);

        public TriggerZoneProperties() { }

        public TriggerZoneProperties(int parentId)
        {
            IsParent = false;
            ParentId = parentId;
            Type = typeof(TriggerZoneProperties).Name;
        }
    }

    public AlwaysFaceCamera IconRotator;
    public MeshRenderer IconGraphic;

    public TriggerZoneProperties Properties { get; set; }

    public TriggerZone Parent { get; set; }
    public bool TriggerZoneActive { get { return Properties.IsParent ? triggerZoneActive : Parent.TriggerZoneActive; } }
    public bool LocalTriggerActive { get; private set; }

    private bool triggerZoneActive;
    private List<TriggerZone> children = new List<TriggerZone>();
    private StateSwitcher stateSwitcher;

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (TriggerZoneProperties)behaviourProperties;

        if (!Properties.IsParent)
        {
            IconGraphic.enabled = false;
            IconRotator.DeActivate();
        }
        else
        {
            stateSwitcher = gameObject.AddComponent<StateSwitcher>();
        }

        BindEvents();
    }

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            IconRotator.DeActivate();
        }
    }

    private void Update()
    {
        if (WorldBuilder.IsInEditor)
        {
            if (Properties != null)
            {
                if (!Properties.IsParent && IconGraphic.enabled)
                {
                    IconGraphic.enabled = false;
                }
                else
                {
                    if (Properties.IsParent && !IconGraphic.enabled)
                        IconGraphic.enabled = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        WorldEditor.Instance.Builder.OnFinishedPlacingBlocks += WorldWasBuilt;
    }

    private void UnBindEvents()
    {
        WorldEditor.Instance.Builder.OnFinishedPlacingBlocks -= WorldWasBuilt;
    }

    private void WorldWasBuilt()
    {
        if (!Properties.IsParent)
        {
            Block parentBlock = WorldEditor.Instance.Builder.GetPlacedBlock(Properties.ParentId);

            if (parentBlock != null)
            {
                Parent = parentBlock.Instance.GetComponent<TriggerZone>();
                if (Parent != null)
                {
                    Parent.AddChild(this);
                    IconGraphic.enabled = false;
                }
                else
                {
                    Debug.LogError("Parent was null for trigger zone");
                }
            }
            else
            {
                Debug.LogError("Parent block was null for trigger zone");
            }
        }
    }

    public void Activated()
    {
        stateSwitcher.Activate();
    }

    public void DeActivated()
    {
        stateSwitcher.Deactivate();
    }

    public void UpdateActiveStatus()
    {
        bool status = LocalTriggerActive;

        foreach (TriggerZone triggerZone in children)
        {
            status = status || triggerZone.LocalTriggerActive;
        }

        if (!triggerZoneActive && status)
            Activated();
        if (triggerZoneActive && !status)
            DeActivated();

        triggerZoneActive = status;
    }

    public void AddChild(TriggerZone triggerZone)
    {
        children.Add(triggerZone);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LocalTriggerActive = true;

            if (!Properties.IsParent)
            {
                if (Parent == null)
                    WorldWasBuilt(); //this is needed because the OnWorldWasBuilt event is only called in the editor for TriggerZone for unknown reasons

                Parent.UpdateActiveStatus();
            }
            else
                UpdateActiveStatus();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LocalTriggerActive = false;

        if (!Properties.IsParent)
        {
            if (Parent == null)
                WorldWasBuilt(); //this is needed because the OnWorldWasBuilt event is only called in the editor for TriggerZone for unknown reasons

            Parent.UpdateActiveStatus();
        }
        else
            UpdateActiveStatus();
    }
}
