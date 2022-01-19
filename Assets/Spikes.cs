using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("spikes");
        if (other.transform.CompareTag("Player"))
        {
            Health health = other.transform.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(1000);
            }
        }
    }
}
