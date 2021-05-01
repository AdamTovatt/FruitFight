using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float Speed = 5f;
    public float Height = 0.5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * Speed) * Height;
        transform.localPosition = new Vector3(startPosition.x, startPosition.y + newY, startPosition.z);
    }
}
