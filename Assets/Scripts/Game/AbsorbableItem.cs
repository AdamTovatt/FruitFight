using UnityEngine;

public class AbsorbableItem : MonoBehaviour
{
    public GameObject SpawnOnAbsorbedPrefab;
    public AbsorbableItemType Type;
    public float AbsorbDelay = 0f;

    private PlayerMovement absorbingPlayer = null;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.transform.GetComponent<PlayerMovement>();
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
    JellyBean
}
