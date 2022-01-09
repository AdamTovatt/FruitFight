using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCamera : ActivatedByStateSwitcher
{
    public GameObject EventCameraGraphic;

    public Vector3 CameraOverridePosition { get { return EventCameraGraphic.transform.position; } }
    public Quaternion CameraOverrideRotation { get { return EventCameraGraphic.transform.rotation; } }

    public Vector3 TargetPosition { get { return _targetPosition; } set { _targetPosition = value; LookAtTarget(); } }
    private Vector3 _targetPosition;
    public bool DeactivateAutomatically { get; set; }
    public float ActiveTime { get; set; }
    public bool ActivateMultipleTimes { get; set; }

    private bool hasBeenActivated;
    private bool isActive;
    private float activateTime;

    private void Start()
    {
        if (TargetPosition != Vector3.zero)
            LookAtTarget();
    }

    private void Update()
    {
        if (isActive && DeactivateAutomatically)
        {
            if (Time.time - activateTime > ActiveTime)
            {
                Deactivated();
            }
        }
    }

    public void LookAtTarget()
    {
        EventCameraGraphic.transform.LookAt(TargetPosition, Vector3.up);
    }

    public override void Activated()
    {
        if (!hasBeenActivated || ActivateMultipleTimes)
        {
            isActive = true;
            hasBeenActivated = true;
            activateTime = Time.time;
            LookAtTarget();
            GameManager.Instance.CameraManager.ShowEventCamera(this);
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
        if (isActive)
        {
            isActive = false;
            GameManager.Instance.CameraManager.StopShowingEventCamera();
            Debug.Log("Event camera was deactivated");
        }
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
