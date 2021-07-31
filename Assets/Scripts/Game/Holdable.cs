using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Holdable : MonoBehaviour
{
    public float Radius = 0.2f;
    public string Id;
    public bool Held { get; private set; }
    public bool HasDetailColor { get; private set; }
    public DetailColor DetailColor { get; private set; }

    private Collider _collider;
    private Rigidbody _rigidbody;

    public delegate void WasPickedUpHandler(Transform pickingTransform);
    public event WasPickedUpHandler OnWasPickedUp;

    private void Start()
    {
        _collider = gameObject.GetComponent<Collider>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();

        HasDetailColor = gameObject.GetComponent<DetailColorController>() != null;
        if (HasDetailColor)
            DetailColor = gameObject.GetComponent<DetailColorController>().Color;
    }

    public void PlacedInHolder(Transform newParent)
    {
        _rigidbody.isKinematic = true;
        transform.parent = newParent;
        transform.localPosition = Vector3.zero;
        Held = true;
    }

    public void WasPickedUp(Transform pickingTransform, Vector3 holdPoint, bool setLayer = true)
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        transform.position = holdPoint;
        transform.parent = pickingTransform;
        Held = true;

        OnWasPickedUp?.Invoke(pickingTransform);
        OnWasPickedUp = null;

        if (setLayer)
            SetLayer(8);
    }

    public void WasDropped(bool setLayer = true)
    {
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
        transform.parent = null;
        Held = false;

        if (setLayer)
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
