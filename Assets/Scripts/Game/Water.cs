using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Material material = new Material(renderer.material);
        Vector3 position = transform.position;
        material.SetVector("_Offset", new Vector4(position.x / -5, position.z / -5, 0, 0));
        renderer.material = material;
    }

    void Update()
    {
        
    }
}
