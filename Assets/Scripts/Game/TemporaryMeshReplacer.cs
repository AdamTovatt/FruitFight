using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryMeshReplacer : NetworkBehaviour
{
    public GameObject ReplacePrefab;
    public GameObject ReplaceEffectPrefab;

    private List<Renderer> renderers = new List<Renderer>();
    private List<Collider> colliders = new List<Collider>();
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private GameObject instantiatedReplacePrefab;

    private void SetRigidbodies()
    {
        rigidbodies.Clear();
        rigidbodies.Add(gameObject.GetComponent<Rigidbody>());
        foreach (Rigidbody _rigidbody in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            if (_rigidbody != null && !_rigidbody.isKinematic)
            {
                rigidbodies.Add(_rigidbody);
            }
        }
    }

    private void SetColliders()
    {
        colliders.Clear();
        colliders.Add(gameObject.GetComponent<Collider>());
        foreach (Collider _collider in gameObject.GetComponentsInChildren<Collider>())
        {
            if (_collider != null && _collider.enabled)
            {
                colliders.Add(_collider);
            }
        }
    }

    private void SetRenderers()
    {
        renderers.Clear();
        renderers.Add(gameObject.GetComponent<Renderer>());
        foreach (Renderer _renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            if (_renderer != null && _renderer.enabled)
                renderers.Add(_renderer);
        }
    }

    public void ReplaceMesh(bool spawnNew, bool setRigidbodyToKinematic = true)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcReplaceMesh(spawnNew, setRigidbodyToKinematic);
            }
            else
            {
                CmdReplaceMesh(spawnNew, setRigidbodyToKinematic);
            }
        }
        else
        {
            PerformMeshReplace(spawnNew, setRigidbodyToKinematic);
        }
    }

    private void PerformMeshReplace(bool spawnNew, bool setRigidbodyToKinematic)
    {
        if (renderers.Count == 0)
            SetRenderers();

        if (colliders.Count == 0)
            SetColliders();

        if (rigidbodies.Count == 0)
            SetRigidbodies();

        if (instantiatedReplacePrefab != null)
            PerformGoBackToNormal();

        foreach (Renderer _renderer in renderers)
        {
            if (_renderer != null)
                _renderer.enabled = false;
        }

        foreach (Collider _collider in colliders)
        {
            if (_collider != null)
                _collider.enabled = false;
        }

        if (setRigidbodyToKinematic)
        {
            foreach (Rigidbody _rigidbody in rigidbodies)
            {
                if (_rigidbody != null)
                    _rigidbody.isKinematic = true;
            }
        }

        if (spawnNew)
        {
            Instantiate(ReplaceEffectPrefab, transform.position, transform.rotation);
            instantiatedReplacePrefab = Instantiate(ReplacePrefab, transform.position, transform.rotation);
        }
    }

    [ClientRpc]
    private void RpcReplaceMesh(bool spawnNew, bool setRigidbodyToKinematic)
    {
        PerformMeshReplace(spawnNew, setRigidbodyToKinematic);
    }

    [Command(requiresAuthority = false)]
    private void CmdReplaceMesh(bool spawnNew, bool setRigidbodyToKinematic)
    {
        RpcReplaceMesh(spawnNew, setRigidbodyToKinematic);
    }

    public void GoBackToNormal()
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcGoBackToNormal();
            }
            else
            {
                CmdGoBackToNormal();
            }
        }
        else
        {
            PerformGoBackToNormal();
        }
    }

    private void PerformGoBackToNormal()
    {
        foreach (Renderer _renderer in renderers)
        {
            if (_renderer != null)
                _renderer.enabled = true;
        }

        foreach (Collider _collider in colliders)
        {
            if (_collider != null)
                _collider.enabled = true;
        }

        foreach (Rigidbody _rigidbody in rigidbodies)
        {
            if (_rigidbody != null)
                _rigidbody.isKinematic = false;
        }

        if (instantiatedReplacePrefab != null)
        {
            Destroy(instantiatedReplacePrefab);
            instantiatedReplacePrefab = null;
        }
    }

    [ClientRpc]
    private void RpcGoBackToNormal()
    {
        PerformGoBackToNormal();
    }

    [Command(requiresAuthority = false)]
    private void CmdGoBackToNormal()
    {
        RpcGoBackToNormal();
    }
}
