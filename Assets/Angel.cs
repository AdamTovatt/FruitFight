using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : MonoBehaviour
{
    public float StartSpeed = 0.5f;
    public float SpeedIncrease = 1f;

    private float speed;

    private void Start()
    {
        speed = StartSpeed;
    }

    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
        speed += SpeedIncrease * Time.deltaTime;
    }
}
