using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWithDelay : MonoBehaviour
{
    public float DelaySeconds = 1f;

    private float spawnTime = 0;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime > DelaySeconds)
            Destroy(gameObject);
    }
}
