using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
    public float Scale = 0.01f;

    private Transform mainCamera;

    private float startTime = 0;

    private void Start()
    {
        startTime = Time.time;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.localPosition = mainCamera.position * Scale;
            transform.rotation = mainCamera.rotation;
        }
        else
        {
            if (Time.time - startTime > 1f)
            {
                Debug.LogError("SkyboxCamera is missing MainCamera reference");
            }
        }
    }

    public void SetMainCamera(Transform mainCamera)
    {
        this.mainCamera = mainCamera;
    }
}
