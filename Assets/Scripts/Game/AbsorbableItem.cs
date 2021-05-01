using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbableItem : MonoBehaviour
{
    public AbsorbableItemType Type;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.transform.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.AbsorbedItem(Type);
            Destroy(gameObject);
        }
    }
}

public enum AbsorbableItemType
{
    JellyBean
}
