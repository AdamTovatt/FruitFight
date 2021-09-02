using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject SpawnOnBreakPrefab;
    [Range(1, 10)]
    public int AmountToSpawn = 1;
    public float SpawnForceStrength = 5f;
    public BoxCollider BoxCollider;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Start()
    {
        if (health != null)
            health.OnDied += WasBroken;
    }

    private void WasBroken(Health sender, CauseOfDeath causeOfDeath)
    {
        BoxCollider.enabled = false;

        if (SpawnOnBreakPrefab != null)
        {
            for (int i = 0; i < AmountToSpawn; i++)
            {
                SpawnPrefab();
            }
        }
    }

    private void SpawnPrefab()
    {
        GameObject instantiatedObject = Instantiate(SpawnOnBreakPrefab, transform.position + (Vector3.up * 0.5f), Quaternion.identity);
        Rigidbody rigidbody = instantiatedObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = Random.onUnitSphere * SpawnForceStrength + Vector3.up;
        }
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDied -= WasBroken;
    }
}
