using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public GameObject ImpactPrefab;
    public Rigidbody Rigidbody;

    private Transform shootingTransform;
    private float shootTime;

    public void Shoot(Transform shooter)
    {
        shootingTransform = shooter;
        Rigidbody.velocity = shooter.forward * 20f;
        shootTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - shootTime > 5f)
        {
            Hit(null);
        }
    }

    private void Hit(Collision collision)
    {
        GameObject impact = Instantiate(ImpactPrefab, collision == null ? transform.position : collision.GetContact(0).point, transform.rotation);

        if (collision != null)
        {
            impact.transform.up = collision.GetContact(0).normal;
            Health health = collision.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(40);
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != this && collision.transform != shootingTransform)
        {
            Hit(collision);
        }
    }
}
