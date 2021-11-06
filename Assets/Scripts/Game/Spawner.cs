using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public bool SpawnInEditor = true;
    public bool SpawnAtStart = true;
    public bool SpawnAtInterval = false;
    public float SecondsBetweenSpawning = 5f;
    public bool SpawnOnNetwork = false;

    public delegate void ObjectSpawnedHandler(object sender, GameObject spawnedObject);
    public ObjectSpawnedHandler OnObjectSpawned;

    private DetailColorController detailColor;

    private void Awake()
    {
        detailColor = gameObject.GetComponent<DetailColorController>();
        if (detailColor == null)
        {
            if (transform.parent != null)
            {
                detailColor = transform.parent.GetComponent<DetailColorController>();
            }
        }
    }

    void Start()
    {
        if (!WorldBuilder.IsInEditor || SpawnInEditor)
        {
            if (SpawnAtStart)
                SpawnObject();

            if (SpawnAtInterval)
                InvokeRepeating("SpawnOneInstance", SecondsBetweenSpawning, SecondsBetweenSpawning);
        }
    }

    public void SpawnOneInstance()
    {
        SpawnObject();
    }

    public GameObject SpawnObject(GameObject theObject = null)
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
            return null;

        if (theObject == null)
            theObject = Prefab;

        GameObject result = Instantiate(theObject, transform.position, transform.rotation);

        if (result.tag == "Player")
            GameManager.Instance.PlayerCharacters.Add(result.GetComponent<PlayerMovement>());

        if (detailColor != null)
        {
            DetailColorController createdDetailColor = result.GetComponent<DetailColorController>();
            if (createdDetailColor != null)
            {
                createdDetailColor.SetEmission(detailColor.CurrentEmission);
                createdDetailColor.Color = detailColor.Color;
                createdDetailColor.StartWithEmission = detailColor.StartWithEmission;
                createdDetailColor.SetTextureFromColor();
            }
        }

        OnObjectSpawned?.Invoke(this, result);

        if (CustomNetworkManager.IsOnlineSession && SpawnOnNetwork)
            NetworkServer.Spawn(result);

        return result;
    }
}
