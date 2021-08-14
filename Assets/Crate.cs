using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject SpawnOnBreakPrefab;
    [Range(1, 10)]
    public int AmountToSpawn = 1;
    public float SpawnForceStrength = 5f;

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
            rigidbody.velocity = (Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.right).normalized * SpawnForceStrength;// + (Vector3.up * (SpawnForceStrength / 3));
        }
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDied -= WasBroken;
    }
}
