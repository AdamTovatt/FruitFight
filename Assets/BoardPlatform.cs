using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlatform : MonoBehaviour
{
    public Transform RotatePoint;
    public Collider Collider;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player" && collision.transform.position.y > collision.transform.position.y)
        {

        }
    }
}
