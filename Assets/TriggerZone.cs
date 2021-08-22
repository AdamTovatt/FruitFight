using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public AlwaysFaceCamera IconRotator;

    public bool IsParent { get; set; }
    public TriggerZone Parent { get; set; }
    public bool TriggerZoneActive { get { return IsParent ? triggerZoneActive : Parent.TriggerZoneActive; } }
    public bool LocalTriggerActive { get; private set; }

    private bool triggerZoneActive;
    private List<TriggerZone> children = new List<TriggerZone>();
    private StateSwitcher stateSwitcher;
    private Block parentBlock;

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            IconRotator.DeActivate();
        }
    }

    public void Init(bool isParent, Block parent)
    {
        IsParent = isParent;

        if (!isParent)
        {
            parentBlock = parent;
        }
    }

    public void Bind()
    {
        if (IsParent)
        {
            Parent = parentBlock.Instance.GetComponent<TriggerZone>();
            Parent.AddChild(this);
        }
        else //is parent
        {
            stateSwitcher = gameObject.AddComponent<StateSwitcher>();
        }
    }

    public void Activated()
    {
        stateSwitcher.Activate();
        Debug.Log("Trigger zone was activated");
    }

    public void DeActivated()
    {
        stateSwitcher.Deactivate();
        Debug.Log("Trigger zone was deactivated");
    }

    public void UpdateActiveStatus()
    {
        bool status = true;

        foreach (TriggerZone triggerZone in children)
        {
            status = status && triggerZone.LocalTriggerActive;
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
        LocalTriggerActive = true;

        if (!IsParent)
            Parent.UpdateActiveStatus();
        else
            UpdateActiveStatus();
    }

    private void OnTriggerExit(Collider other)
    {
        LocalTriggerActive = false;

        if (!Parent)
            Parent.UpdateActiveStatus();
        else
            UpdateActiveStatus();
    }
}
