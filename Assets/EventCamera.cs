using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCamera : ActivatedByStateSwitcher
{
    public Vector3 TargetPosition { get { return _targetPosition; } set { _targetPosition = value; LookAtTarget(); } }
    private Vector3 _targetPosition;
    public bool DeactivateAutomatically { get; set; }
    public float ActiveTime { get; set; }
    public bool ActivateMultipleTimes { get; set; }

    private bool hasBeenActivated;

    private void Start()
    {
        if (TargetPosition != Vector3.zero)
            LookAtTarget();
    }

    private void LookAtTarget()
    {
        transform.LookAt(TargetPosition, Vector3.up);
    }

    public override void Activated()
    {
        if (!hasBeenActivated || ActivateMultipleTimes)
        {
            hasBeenActivated = true;
            Debug.Log("Event Camera was activated");
        }
    }

    public override void BindStateSwitcher()
    {
        if (activatorObject != null && activatorObject.Instance != null)
        {
            stateSwitcher = activatorObject.Instance.GetComponent<StateSwitcher>();
            if (stateSwitcher != null)
            {
                stateSwitcher.OnActivated += Activated;
                stateSwitcher.OnDeactivated += Deactivated;
            }
            else
            {
                Debug.Log("StateSwitcher was null: " + transform.name);
            }
        }
    }

    public override void Deactivated()
    {
        Debug.Log("Event camera was activated");
    }

    public override void Init(Block thisBlock, Block activatorBlock)
    {
        block = thisBlock;
        activatorObject = activatorBlock;
    }

    private void OnDestroy()
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated -= Activated;
            stateSwitcher.OnDeactivated -= Deactivated;
        }
    }
}
