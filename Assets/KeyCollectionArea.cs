using System.Collections.Generic;
using UnityEngine;

public class KeyCollectionArea : MonoBehaviour
{
    public StateSwitcher StateSwitcher;
    public Transform HoldPoint;
    public string KeyId;

    private Dictionary<int, Holdable> holdableItems = new Dictionary<int, Holdable>();

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Interactable")
        {
            int instanceId = other.GetInstanceID();
            if (!holdableItems.ContainsKey(other.GetInstanceID()))
            {
                Debug.Log("Did holdable lookup");
                holdableItems.Add(instanceId, other.GetComponent<Holdable>());
            }

            Holdable holdable = holdableItems[instanceId];

            if(holdable != null && !holdable.Held)
            {
                if(holdable.Id == KeyId)
                {
                    holdable.PlacedInHolder(HoldPoint);
                    StateSwitcher.Activate();
                    holdable.OnWasPickedUp += KeyRemoved;
                }
            }
        }
    }

    private void KeyRemoved(Transform removingTransform)
    {
        StateSwitcher.Deactivate();
    }
}
