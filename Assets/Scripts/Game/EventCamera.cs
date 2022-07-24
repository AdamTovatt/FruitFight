using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCamera : BehaviourBase
{
    public class EventCameraProperties : BehaviourProperties
    {
        [ActivatorInput(Name = "Activator", Description = "The thing that should activate this event camera")]
        public int ActivatorId { get; set; }

        [BoolInput(Name = "Auto-deactivate", Description = "If the camera should deactivate automatically after a delay when it has been activated. This should generally be set to yes")]
        public bool DeactivateAutomatically { get; set; }

        [FloatInput(Name = "Active time", Description = "The time the camera will remain active for if using auto-deactivate", MinValue = 0, MaxValue = 60)]
        public float ActiveTime { get; set; }

        [BoolInput(Name = "Multiple activations", Description = "If the camera should be able to be activated multiple times or not")]
        public bool ActivateMultipleTimes { get; set; }

        [PositionInput(Name = "Target", Description = "The target position. The camera will look at this position")]
        public Vector3Int TargetPosition { get; set; }

        public override Type BehaviourType => typeof(EventCamera);
    }

    public GameObject EventCameraGraphic;

    public Vector3 CameraOverridePosition { get { return EventCameraGraphic.transform.position; } }
    public Quaternion CameraOverrideRotation { get { return EventCameraGraphic.transform.rotation; } }

    public EventCameraProperties Properties { get; set; }

    private StateSwitcher stateSwitcher;
    private bool hasBeenActivated;
    private bool isActive;
    private float activateTime;

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (EventCameraProperties)behaviourProperties;
        BindEvents();
        LookAtTarget();
    }

    private void Update()
    {
        if (isActive)
        {
            LookAtTarget();

            if (Properties.DeactivateAutomatically && Time.time - activateTime > Properties.ActiveTime)
            {
                Deactivated();
            }
        }
    }

    private void BindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks += WorldWasBuilt;
    }

    private void UnBindEvents()
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated -= Activated;
            stateSwitcher.OnDeactivated -= Deactivated;
        }

        WorldBuilder.Instance.OnFinishedPlacingBlocks -= WorldWasBuilt;
    }

    private void WorldWasBuilt()
    {
        Block block = WorldBuilder.Instance.GetPlacedBlock(Properties.ActivatorId);

        if (block != null)
            stateSwitcher = block.Instance.GetComponent<StateSwitcher>();

        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated += Activated;
            stateSwitcher.OnDeactivated += Deactivated;
        }
    }

    public void LookAtTarget()
    {
        if (Properties.TargetPosition != null)
            EventCameraGraphic.transform.LookAt(Properties.TargetPosition, Vector3.up);
    }

    private void Activated()
    {
        if (!hasBeenActivated || Properties.ActivateMultipleTimes)
        {
            isActive = true;
            hasBeenActivated = true;
            activateTime = Time.time;
            LookAtTarget();
            GameManager.Instance.CameraManager.ShowEventCamera(this);
            Debug.Log("Event Camera was activated");
        }
    }

    private void Deactivated()
    {
        if (isActive)
        {
            isActive = false;
            GameManager.Instance.CameraManager.StopShowingEventCamera();
            Debug.Log("Event camera was deactivated");
        }
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }
}
