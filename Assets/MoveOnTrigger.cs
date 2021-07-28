using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : MonoBehaviour
{
    public Vector3 FinalPosition { get; set; }
    public int ActivatorBlockId { get; set; }

    private Block activatorObject;
    private Block block;

    private StateSwitcher stateSwitcher;

    public void Init(Block thisBlock, Block activatorBlock)
    {
        block = thisBlock;
        activatorObject = activatorBlock;
    }

    public void BindStateSwitcher()
    {
        stateSwitcher = activatorObject.Instance.GetComponent<StateSwitcher>();
        stateSwitcher.OnActivated += Activated;
        stateSwitcher.OnDeactivated += Deactivated;
    }

    public void Activated()
    {
        transform.position = FinalPosition;
    }

    public void Deactivated()
    {
        transform.position = block.Position;
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
