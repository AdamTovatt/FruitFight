using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationBlock : ActivatedByStateSwitcher
{
    public AlwaysFaceCamera IconRotator;
    public MeshRenderer IconGraphic;

    public string IconName { get; set; }
    public string Text { get; set; }
    public int DisplayTime { get; set; }
    public bool ShowMultipleTimes { get; set; }

    private bool hasBeenActivated = false;

    public override void Activated()
    {
        if (!hasBeenActivated || ShowMultipleTimes)
        {
            hasBeenActivated = true;
            AlertCreator.Instance.CreateNotification(Text, DisplayTime, IconName);
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
        //notifications don't really do anything when deactivated
    }

    public override void Init(Block thisBlock, Block activatorBlock)
    {
        block = thisBlock;
        activatorObject = activatorBlock;
    }

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            IconRotator.DeActivate();
        }
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
