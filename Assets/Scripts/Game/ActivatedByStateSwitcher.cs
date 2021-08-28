using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatedByStateSwitcher : MonoBehaviour
{
    public int ActivatorBlockId { get; set; }

    protected Block activatorObject;
    protected Block block;
    protected StateSwitcher stateSwitcher;

    public abstract void Init(Block thisBlock, Block activatorBlock);

    public abstract void BindStateSwitcher();

    public abstract void Activated();

    public abstract void Deactivated();
}
