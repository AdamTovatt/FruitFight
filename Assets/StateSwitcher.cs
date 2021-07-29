using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwitcher : MonoBehaviour
{
    public bool HasDetailColor { get; private set; }
    public DetailColor DetailColor { get; private set; }

    public delegate void OnActivatedHandler();
    public event OnActivatedHandler OnActivated;

    public delegate void OnDeactivatedHandler();
    public event OnDeactivatedHandler OnDeactivated;

    private void Start()
    {
        HasDetailColor = gameObject.GetComponent<DetailColorController>() != null;
        if (HasDetailColor)
            DetailColor = gameObject.GetComponent<DetailColorController>().Color;
    }

    public void Activate()
    {
        Debug.Log("State switcher was activated");
        OnActivated?.Invoke();
    }

    public void Deactivate()
    {
        Debug.Log("State switcher was deactivated");
        OnDeactivated?.Invoke();
    }
}
