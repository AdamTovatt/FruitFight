using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SkyboxCamera : MonoBehaviour
{
    public float Scale = 0.01f;

    private Transform mainCamera;
    private Camera _camera;
    private MultipleTargetCamera multipleTargetCamera;

    private float startTime = 0;

    private void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
        startTime = Time.time;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.localPosition = mainCamera.position * Scale;
            transform.rotation = mainCamera.rotation;

            if (multipleTargetCamera != null)
                _camera.fieldOfView = multipleTargetCamera.FieldOfView;
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
        multipleTargetCamera = mainCamera.gameObject.GetComponent<MultipleTargetCamera>();
    }
}
