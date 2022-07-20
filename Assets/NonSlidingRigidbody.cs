using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NonSlidingRigidbody : MonoBehaviour
{
    public static Dictionary<Transform, Rigidbody> Rigidbodies { get; set; } = new Dictionary<Transform, Rigidbody>();
    private Collider collider;

    private void Awake()
    {
        collider = gameObject.GetComponent<Collider>();
    }

    private void Update()
    {
        Rigidbody rigidbody = GetGroundRigidbody();

        if (rigidbody != null)
            Debug.Log(rigidbody.transform.name);
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
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if ((collider.ClosestPoint(hit.point) - hit.point).sqrMagnitude < 0.1f)
                return hit.transform;
        }

        return null;
    }
}
