using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Holdable : NetworkBehaviour
{
    public float Radius = 0.2f;
    public string Id;
    public GameObject DummyPrefab;
    public TemporaryMeshReplacer MeshReplacer;

    public Vector3 HeldPosition { get { if (instantiatedDummyObject != null) return instantiatedDummyObject.transform.position; return transform.position; } }
    public bool Held { get; private set; }
    public bool InHolder { get; private set; }
    public bool HasDetailColor { get; private set; }
    public DetailColor DetailColor { get; private set; }

    private Collider _collider;
    private Rigidbody _rigidbody;

    public delegate void WasPickedUpHandler(Transform pickingTransform);
    public event WasPickedUpHandler OnWasPickedUp;

    private DetailColorController detailColorController;

    private SoundSource soundSource;
    private Vector3 lastCollisionPoint;

    private GameObject instantiatedDummyObject;

    private Transform lastPickingTransform;
    private Transform holdingTransform;
    private Player holdingPlayer;
    private Transform holderTransform;

    private void Start()
    {
        _collider = gameObject.GetComponent<Collider>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        soundSource = gameObject.GetComponent<SoundSource>();

        detailColorController = gameObject.GetComponent<DetailColorController>();
        HasDetailColor = detailColorController != null;
        if (HasDetailColor)
            DetailColor = detailColorController.Color;
    }

    private void Update()
    {
        if (Held && CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer)
        {
            if (instantiatedDummyObject != null)
                transform.position = instantiatedDummyObject.transform.position;
        }

        if (InHolder)
        {
            if (holderTransform != null)
                transform.position = holderTransform.position;
        }
    }

    public void PlacedInHolder(Transform holder)
    {
        if (_rigidbody == null)
            _rigidbody = gameObject.GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        holderTransform = holder;
        InHolder = true;
        Held = true;
    }

    public void WasPickedUp(Transform pickingTransform, bool setLayer = true)
    {
        lastPickingTransform = pickingTransform;

        if (!CustomNetworkManager.IsOnlineSession) //this is an offline session
        {
            PerformPickUp(false, transform.rotation, setLayer);
        }
        else //this is an online session
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcPickup(false, transform.rotation, setLayer);
            }
            else
            {
                CmdPickup(true, transform.rotation, setLayer);
            }
        }

        MeshReplacer.ReplaceMesh(false);
    }

    [Command(requiresAuthority = false)]
    private void CmdPickup(bool pickedByClient, Quaternion rotation, bool setLayer)
    {
        RpcPickup(pickedByClient, rotation, setLayer);
    }

    [ClientRpc]
    private void RpcPickup(bool pickedByClient, Quaternion rotation, bool setLayer)
    {
        PerformPickUp(pickedByClient, rotation, setLayer);
    }

    private void PerformPickUp(bool pickedByClient, Quaternion rotation, bool setLayer)
    {
        lastCollisionPoint = Vector3.zero;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        Held = true;
        InHolder = false;
        holderTransform = null;

        if (instantiatedDummyObject != null)
        {
            Destroy(instantiatedDummyObject);
        }

        Transform pickingTransform = GetPickingTransform(pickedByClient);
        holdingTransform = pickingTransform;

        holdingPlayer = holdingTransform.gameObject.GetComponentInParent<Player>();

        instantiatedDummyObject = Instantiate(DummyPrefab, holdingPlayer.Movement.HoldPoint, rotation);
        instantiatedDummyObject.transform.parent = pickingTransform;

        if(HasDetailColor)
        {
            DetailColorController dummyDetailColor = instantiatedDummyObject.GetComponent<DetailColorController>();
            if(dummyDetailColor != null)
            {
                dummyDetailColor.Color = DetailColor;
                dummyDetailColor.SetTextureFromColor();
            }
        }

        if (setLayer)
            SetLayerForObject(instantiatedDummyObject, 8);

        OnWasPickedUp?.Invoke(pickingTransform);
        OnWasPickedUp = null;

        holdingPlayer.PickedUpItem(this);

        Debug.Log("perform pickup: " + instantiatedDummyObject);
    }

    public void WasDropped(Vector3 holderVelocity, float holdingBodyMovingVelocity)
    {
        Debug.Log("Wass dropped");
        if (!CustomNetworkManager.IsOnlineSession) //this is an offline session
        {
            PerformDrop(holderVelocity, holdingBodyMovingVelocity);
        }
        else //this is an online session
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcDrop(holderVelocity, holdingBodyMovingVelocity);
            }
            else
            {
                CmdDrop(holderVelocity, holdingBodyMovingVelocity);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdDrop(Vector3 holderVelocity, float holdingBodyMovingVelocity)
    {
        RpcDrop(holderVelocity, holdingBodyMovingVelocity);
    }

    [ClientRpc]
    private void RpcDrop(Vector3 holderVelocity, float holdingBodyMovingVelocity)
    {
        PerformDrop(holderVelocity, holdingBodyMovingVelocity);
    }

    public void PerformDrop(Vector3 holderVelocity, float holdingBodyMovingVelocity)
    {
        Debug.Log("was dropped");
        transform.position = instantiatedDummyObject.transform.position;
        transform.parent = null;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = holderVelocity;
        _rigidbody.AddForce(holdingTransform.forward * holdingBodyMovingVelocity, ForceMode.Impulse);
        _collider.enabled = true;
        Held = false;

        MeshReplacer.GoBackToNormal();
        Destroy(instantiatedDummyObject);

        holdingPlayer.DroppedItem();
    }

    private void SetLayerForObject(GameObject theObject, int layerIndex)
    {
        List<GameObject> family = new List<GameObject>() { theObject };
        theObject.GetComponentsInChildren<Transform>().ToList().ForEach(x => family.Add(x.gameObject));

        foreach (GameObject member in family)
        {
            member.layer = layerIndex;
        }
    }

    private Transform GetPickingTransform(bool pickedByClient)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (pickedByClient)
            {
                if (CustomNetworkManager.Instance.IsServer)
                {
                    return PlayerNetworkCharacter.OtherPlayer.PlayerMovement.SpineTransform;
                }
                else
                {
                    return PlayerNetworkCharacter.LocalPlayer.PlayerMovement.SpineTransform;
                }
            }
            else
            {
                if (CustomNetworkManager.Instance.IsServer)
                {
                    return PlayerNetworkCharacter.LocalPlayer.PlayerMovement.SpineTransform;
                }
                else
                {
                    return PlayerNetworkCharacter.OtherPlayer.PlayerMovement.SpineTransform;
                }
            }
        }
        else
        {
            return lastPickingTransform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (soundSource != null)
        {
            if (collision.transform.tag != "Player")
            {
                if (lastCollisionPoint == Vector3.zero || Mathf.Abs(collision.transform.position.y - lastCollisionPoint.y) > 0.2f)
                {
                    soundSource.Play("groundHit");
                    lastCollisionPoint = collision.transform.position;
                }
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Holdable was destoryed");
    }
}
