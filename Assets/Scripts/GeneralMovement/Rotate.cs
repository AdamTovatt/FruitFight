using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float Speed = 1f;

    public bool X = false;
    public bool Y = true;
    public bool Z = false;

    void Update()
    {
        if (X)
            transform.Rotate(Speed * Time.deltaTime, 0, 0, Space.World);
        if (Y)
            transform.Rotate(0, Speed * Time.deltaTime, 0, Space.World);
        if (Z)
            transform.Rotate(0, 0, Speed * Time.deltaTime, Space.World);
    }

    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
