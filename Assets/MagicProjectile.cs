using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public GameObject ImpactPrefab;
    public Rigidbody Rigidbody;

    private Transform shootingTransform;
    private float shootTime;

    public void Shoot(Vector3 shootOrigin, Transform shooter)
    {
        Health targetHealth = null;
        float closestTargetHealhtDistance = 10000f;

        if (GameManager.Instance != null)
        {
            Ray sphereCastRay = new Ray(shootOrigin, transform.forward);
            foreach (RaycastHit hit in Physics.SphereCastAll(sphereCastRay, 2))
            {
                if (hit.transform != shooter)
                {
                    if (GameManager.Instance.TransformsWithHealth.ContainsKey(hit.transform))
                    {
                        if (hit.distance < closestTargetHealhtDistance)
                            targetHealth = GameManager.Instance.TransformsWithHealth[hit.transform];
                    }
                }
            }
        }

        Vector3 shootDirection = shooter.forward;

        if (targetHealth != null)
        {
            Vector3 targetPosition = new Vector3(targetHealth.transform.position.x, shootOrigin.y, targetHealth.transform.position.z);
            shootDirection = (targetPosition - shootOrigin).normalized;
        }

        shootingTransform = shooter;
        Rigidbody.velocity = shootDirection * 20f;
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
