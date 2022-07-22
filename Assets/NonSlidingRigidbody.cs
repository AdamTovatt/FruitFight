using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class NonSlidingRigidbody : MonoBehaviour
{
    public static Dictionary<Transform, Rigidbody> Rigidbodies { get; set; } = new Dictionary<Transform, Rigidbody>();
    private Collider collider;
    private Rigidbody rigidbody;

    private void Awake()
    {
        collider = gameObject.GetComponent<Collider>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Rigidbody groundRigidBody = GetGroundRigidbody();

        if (groundRigidBody != null)
        {
            Debug.Log(groundRigidBody.velocity);
            rigidbody.MovePosition(rigidbody.transform.position + groundRigidBody.velocity * Time.deltaTime);
        }
        else
            Debug.Log("nothing");
    }

    private Rigidbody GetGroundRigidbody()
    {
        Transform ground = GetGroundTransform();

        if(ground != null)
        {
            if (Rigidbodies.TryGetValue(ground, out Rigidbody rigidbody))
                return rigidbody;
        }

        return null;
    }

    private Transform GetGroundTransform()
    {
        Ray ray = new Ray(collider.bounds.center, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if ((collider.ClosestPoint(hit.point) - hit.point).sqrMagnitude < 0.1f)
                return hit.transform;
        }

        return null;
    }
}
