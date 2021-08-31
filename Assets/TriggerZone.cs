using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public AlwaysFaceCamera IconRotator;
    public MeshRenderer IconGraphic;

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
            IconGraphic.enabled = false;
            IconRotator.DeActivate();
        }
        else
        {
            stateSwitcher = gameObject.AddComponent<StateSwitcher>();
        }
    }

    public void Bind()
    {
        if (!IsParent)
        {
            Parent = parentBlock.Instance.GetComponent<TriggerZone>();
            if (Parent != null)
                Parent.AddChild(this);
            else
                Debug.Log("Parent was null");
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

            if (!IsParent)
                Parent.UpdateActiveStatus();
            else
                UpdateActiveStatus();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LocalTriggerActive = false;

        if (!IsParent)
            Parent.UpdateActiveStatus();
        else
            UpdateActiveStatus();
    }
}
