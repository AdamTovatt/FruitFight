using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;

    void Start()
    {
        SpawnObject();
    }

    public GameObject SpawnObject(GameObject theObject = null)
    {
        if (theObject == null)
            theObject = Prefab;

        GameObject result = Instantiate(theObject, transform.position, transform.rotation);

        if (result.tag == "Player")
            GameManager.Instance.Players.Add(result.GetComponent<PlayerMovement>());

        return result;
    }
}
