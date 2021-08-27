using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatedByStateSwitcher : MonoBehaviour
{
    public int ActivatorBlockId { get; set; }

    private Block activatorObject;
    private Block block;

    public abstract void Init(Block thisBlock, Block activatorBlock);

    public abstract void BindStateSwitcher();

    public abstract void Activated();

    public abstract void Deactivated();
}
