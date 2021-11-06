using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject SpawnOnBreakPrefab;
    [Range(1, 10)]
    public int AmountToSpawn = 1;
    public float SpawnForceStrength = 5f;
    public BoxCollider BoxCollider;
    public Rigidbody Rigidbody;

    private Health health;

    private bool isFalling;
    private bool wasOnGroundLastCheck;
    private float lastGroundCheck;

    private Dictionary<Transform, Crate> crateLookup = new Dictionary<Transform, Crate>();

    private void Awake()
    {
        health = GetComponent<Health>();

        if (WorldEditor.Instance == null || WorldEditor.IsTestingLevel)
        {
            if (!StartFalling())
            {
                Rigidbody.isKinematic = true;
            }
        }
    }

    private void Update()
    {
        if (isFalling)
        {
            if (Time.time - lastGroundCheck > 0.5f)
            {
                if (CheckIfOnGround())
                {
                    if (!wasOnGroundLastCheck)
                    {
                        wasOnGroundLastCheck = true;
                    }
                    else
                    {
                        isFalling = false;
                        Rigidbody.isKinematic = true;
                        wasOnGroundLastCheck = false;
                    }
                }

                lastGroundCheck = Time.time;
            }
        }
    }

    private void Start()
    {
        if (health != null)
            health.OnDied += WasBroken;
    }

    private void WasBroken(Health sender, CauseOfDeath causeOfDeath)
    {
        BoxCollider.enabled = false;

        if (SpawnOnBreakPrefab != null && CustomNetworkManager.HasAuthority)
        {
            for (int i = 0; i < AmountToSpawn; i++)
            {
                GameObject spawnedObject = SpawnPrefab();

                if (CustomNetworkManager.IsOnlineSession)
                    NetworkServer.Spawn(spawnedObject);
            }
        }

        ActivateCrateAbove();
    }

    private GameObject SpawnPrefab()
    {
        GameObject instantiatedObject = Instantiate(SpawnOnBreakPrefab, transform.position + (Vector3.up * 0.5f), Quaternion.identity);
        Rigidbody rigidbody = instantiatedObject.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.velocity = Random.onUnitSphere * SpawnForceStrength + Vector3.up;
        }

        return instantiatedObject;
    }

    private void ActivateCrateAbove()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up, 0.1f);

        Collider collider = colliders.Where(x => x.transform != transform && x.transform.tag == "Ground").FirstOrDefault();

        if (collider != null)
        {
            Crate crate = collider.transform.GetComponent<Crate>();
            if (crate != null)
            {
                crate.StartFalling();
            }
        }
    }

    private bool CheckIfOnGround()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);

        foreach(Collider collider in colliders)
        {
            if (!crateLookup.ContainsKey(collider.transform))
                crateLookup.Add(collider.transform, collider.transform.GetComponent<Crate>());
        }

        if (colliders.Any(x => x.transform != transform && x.transform.tag == "Ground" && (crateLookup[x.transform] == null || !crateLookup[x.transform].isFalling)))
        {
            return true;
        }

        return false;
    }

    public bool StartFalling()
    {
        if (!CheckIfOnGround())
        {
            isFalling = true;
            Rigidbody.isKinematic = false;
            Rigidbody.WakeUp();

            ActivateCrateAbove();
            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDied -= WasBroken;
    }
}
