using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwitcher : MonoBehaviour
{
    public bool HasDetailColor { get; private set; }
    public DetailColor DetailColor { get; private set; }

    public bool IsActive { get; private set; }

    public delegate void OnActivatedHandler();
    public event OnActivatedHandler OnActivated;

    public delegate void OnDeactivatedHandler();
    public event OnDeactivatedHandler OnDeactivated;

    private HoldableDelegate holdableDelegate;

    private void Awake()
    {
        holdableDelegate = gameObject.GetComponent<HoldableDelegate>();
    }

    private void Start()
    {
        HasDetailColor = gameObject.GetComponent<DetailColorController>() != null;
        if (HasDetailColor)
            DetailColor = gameObject.GetComponent<DetailColorController>().Color;
    }

    public void Activate()
    {
        Activate(null);
    }

    public void Activate(Holdable holdable)
    {
        IsActive = true;

        if (holdable != null)
        {
            if (holdableDelegate != null)
                holdableDelegate.SetContainedHoldable(holdable);
        }

        OnActivated?.Invoke();
    }

    public void Deactivate()
    {
        IsActive = false;

        if (holdableDelegate != null)
            holdableDelegate.RemoveContainedHoldable();

        OnDeactivated?.Invoke();
    }
}
