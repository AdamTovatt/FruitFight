using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryMeshReplacer : NetworkBehaviour
{
    public GameObject ReplacePrefab;
    public GameObject ReplaceEffectPrefab;

    private List<Renderer> renderers = new List<Renderer>();
    private GameObject instantiatedReplacePrefab;

    private void Start()
    {
        renderers.Add(gameObject.GetComponent<Renderer>());
        foreach (Renderer _renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            if (_renderer != null && _renderer.enabled)
                renderers.Add(_renderer);
        }
    }

    public void ReplaceMesh()
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcReplaceMesh();
            }
            else
            {
                CmdReplaceMesh();
            }
        }
        else
        {
            PerformMeshReplace();
        }
    }

    private void PerformMeshReplace()
    {
        if (instantiatedReplacePrefab != null)
            PerformGoBackToNormal();

        foreach (Renderer _renderer in renderers)
        {
            if (_renderer != null)
                _renderer.enabled = false;
        }

        Instantiate(ReplaceEffectPrefab, transform.position, transform.rotation);
        instantiatedReplacePrefab = Instantiate(ReplacePrefab, transform.position, transform.rotation);
    }

    [ClientRpc]
    private void RpcReplaceMesh()
    {
        PerformMeshReplace();
    }

    [Command(requiresAuthority = false)]
    private void CmdReplaceMesh()
    {
        RpcReplaceMesh();
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
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        Destroy(instantiatedReplacePrefab);
        instantiatedReplacePrefab = null;
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
