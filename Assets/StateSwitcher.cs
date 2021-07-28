using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwitcher : MonoBehaviour
{
    public delegate void OnActivatedHandler();
    public event OnActivatedHandler OnActivated;

    public delegate void OnDeactivatedHandler();
    public event OnDeactivatedHandler OnDeactivated;

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
