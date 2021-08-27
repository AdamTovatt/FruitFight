using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationBlock : ActivatedByStateSwitcher
{
    public AlwaysFaceCamera IconRotator;
    public MeshRenderer IconGraphic;

    public string IconName { get; set; }
    public string Text { get; set; }
    public float DisplayTime { get; set; }

    public override void Activated()
    {
        AlertCreator.GetInstance().CreateNotification(Text, DisplayTime, IconName);
    }

    public override void BindStateSwitcher()
    {
        throw new System.NotImplementedException();
    }

    public override void Deactivated()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(Block thisBlock, Block activatorBlock)
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
        {
            IconRotator.DeActivate();
        }
        AlertCreator.GetInstance().CreateNotification("Test not", 1);
    }
}
