using UnityEngine;

public class AbsorbableItem : MonoBehaviour
{
    public GameObject SpawnOnAbsorbedPrefab;
    public AbsorbableItemType Type;
    public float AbsorbDelay = 0f;

    private PlayerInventory absorbingPlayer = null;

    private void OnTriggerEnter(Collider other)
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
            return;

        PlayerInventory player = other.transform.GetComponent<PlayerInventory>();
        if (player != null && absorbingPlayer == null)
        {
            absorbingPlayer = player;

            if (SpawnOnAbsorbedPrefab != null)
                Instantiate(SpawnOnAbsorbedPrefab, transform.position, Quaternion.identity);

            this.CallWithDelay(Absorb, AbsorbDelay);
        }
    }

    private void Absorb()
    {
        absorbingPlayer.AbsorbedItem(Type);
        Destroy(gameObject);
    }
}

public enum AbsorbableItemType
{
    JellyBean, Coin
}
