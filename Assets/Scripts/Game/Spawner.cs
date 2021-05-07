using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public bool SpawnAtStart = true;
    public bool SpawnAtInterval = false;
    public float SecondsBetweenSpawning = 5f;

    void Start()
    {
        if (SpawnAtStart)
            SpawnObject();

        if (SpawnAtInterval)
            InvokeRepeating("SpawnOneInstance", SecondsBetweenSpawning, SecondsBetweenSpawning);
    }

    public void SpawnOneInstance()
    {
        SpawnObject();
    }

    public GameObject SpawnObject(GameObject theObject = null)
    {
        if (theObject == null)
            theObject = Prefab;

        GameObject result = Instantiate(theObject, transform.position, transform.rotation);

        if (result.tag == "Player")
            GameManager.Instance.PlayerCharacters.Add(result.GetComponent<PlayerMovement>());

        return result;
    }
}
