using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Holdable : MonoBehaviour
{
    private Collider _collider;
    private Rigidbody _rigidbody;

    public float Radius = 0.2f;

    private void Start()
    {
        _collider = gameObject.GetComponent<Collider>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void WasPickedUp(Transform pickingTransform, Vector3 holdPoint)
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        transform.position = holdPoint;
        transform.parent = pickingTransform;
        SetLayer(8);
    }

    public void WasDropped()
    {
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
        transform.parent = null;
        SetLayer(0);
    }

    private void SetLayer(int layerIndex)
    {
        List<GameObject> family = new List<GameObject>() { gameObject };
        gameObject.GetComponentsInChildren<Transform>().ToList().ForEach(x => family.Add(x.gameObject));

        foreach (GameObject member in family)
        {
            member.layer = layerIndex;
        }
    }
}
