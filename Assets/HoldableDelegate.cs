using UnityEngine;

public class HoldableDelegate : MonoBehaviour
{
    public Holdable ContainedHoldable;

    public void SetContainedHoldable(Holdable holdable)
    {
        ContainedHoldable = holdable;
    }

    public void RemoveContainedHoldable()
    {
        ContainedHoldable = null;
    }
}
