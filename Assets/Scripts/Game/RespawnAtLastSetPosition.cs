using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAtLastSetPosition : MonoBehaviour
{
    public GameObject WaterSplash;
    public GameObject SmokePoof;

    private Vector3 LastPosition;

    private void Start()
    {
        LastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Water")
        {
            Instantiate(WaterSplash, transform.position, Quaternion.Euler(-90, 0, 0));
            transform.position = LastPosition;
            Instantiate(SmokePoof, LastPosition, Quaternion.identity);
        }
    }

    public void SetLastPosition()
    {
        LastPosition = transform.position;
    }
}
