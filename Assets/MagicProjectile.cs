using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float Scale = 1f;
    public GameObject ImpactPrefab;
    public Rigidbody Rigidbody;

    private Transform shootingTransform;
    private Vector3 origin;
    private float shootTime;
    private Transform target;
    private Vector3 targetPositionHeightOffset;

    public void Shoot(Vector3 shootOrigin, Transform shooter, Vector3 shooterForward, Vector3 shooterPosition)
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in children)
        {
            childTransform.localScale = new Vector3(Scale, Scale, Scale);
        }

        Health targetHealth = null;
        float smallestAngle = 10000f;

        if (GameManager.Instance != null)
        {
            Ray sphereCastRay = new Ray(shootOrigin, shooterForward);
            foreach (RaycastHit hit in Physics.SphereCastAll(sphereCastRay, 6))
            {
                if (hit.transform.tag != "Player" && hit.transform != shooter)
                {
                    if (GameManager.Instance.TransformsWithHealth.ContainsKey(hit.transform))
                    {
                        float angle = Vector3.Angle(shooterForward, (hit.transform.position - transform.position).normalized);
                        if (angle < 45f)
                        {
                            Ray visibleRay = new Ray(shootOrigin, (hit.transform.position + Vector3.up * 0.1f) - shootOrigin);
                            bool targetVisible = true;

                            if (Physics.Raycast(visibleRay, out RaycastHit visibleCheckHit, 10000, ~(1 << 2), QueryTriggerInteraction.Ignore))
                            {
                                if (visibleCheckHit.transform != hit.transform)
                                {
                                    targetVisible = false;
                                }
                            }

                            if (targetVisible && angle < smallestAngle) //if nothing seems to be hit targetVisible check could be removed
                            {
                                targetHealth = GameManager.Instance.TransformsWithHealth[hit.transform];
                                target = targetHealth.transform;
                                smallestAngle = angle;
                            }
                        }
                    }
                }
            }
        }

        Vector3 shootDirection = shooterForward;

        if (targetHealth != null)
        {
            Vector3 targetPosition = new Vector3(targetHealth.transform.position.x, shootOrigin.y, targetHealth.transform.position.z);
            shootDirection = (targetPosition - shootOrigin).normalized;
        }

        shootingTransform = shooter;
        origin = shootOrigin;

        Rigidbody.velocity = shootDirection * 20f;
        shootTime = Time.time;

        if (target != null)
        {
            targetPositionHeightOffset = target.GetComponent<Collider>().bounds.center - target.position;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + targetPositionHeightOffset;
            Vector3 targetDirection = targetPosition - transform.position;
            if (Vector3.Angle(targetDirection, Rigidbody.velocity) < 15)
            {
                Rigidbody.velocity = Vector3.RotateTowards(Rigidbody.velocity, (targetPosition - transform.position).normalized, 3f * Time.deltaTime, 0);
            }
        }

        Rigidbody.velocity += ((Rigidbody.velocity * 0.1f) * Time.deltaTime);

        if (Time.time - shootTime > 5f)
        {
            Hit(null);
        }
    }

    private void Hit(Collision collision)
    {
        Vector3 impactPoint = collision == null ? transform.position : collision.GetContact(0).point;
        GameObject impact = Instantiate(ImpactPrefab, impactPoint, transform.rotation);

        if (collision != null)
        {
            impact.transform.up = collision.GetContact(0).normal;
            Health health = collision.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(40);

                if (health.MovingCharacter != null)
                {
                    health.MovingCharacter.WasAttacked(impactPoint + (shootingTransform.position - health.transform.position).normalized * 0.5f, shootingTransform, 5);
                }
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
