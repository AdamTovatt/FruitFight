using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public bool StartActive;
    public bool Active { get { return active; } set { SetActive(value); } }
    private bool active;

    private Transform cameraTransform;

    private void Start()
    {
        active = StartActive;

        if (active)
            cameraTransform = GetCameraTransform();
    }

    private void Update()
    {
        if (active)
        {
            if (cameraTransform == null)
                cameraTransform = GetCameraTransform();

            if (cameraTransform != null)
            {
                transform.LookAt(cameraTransform);
            }
            else
            {
                Debug.LogError("Camera transform is null, " + transform.name + " can't look at camera");
            }
        }
    }

    private Transform GetCameraTransform()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();

        if (cameras != null && cameras.Length > 0)
        {
            foreach (Camera camera in cameras)
            {
                if (camera.transform.tag == "MainCamera")
                    return camera.transform;
            }
        }

        return null;
    }

    public void SetActive(bool newValue)
    {
        if (newValue)
        {
            active = true;
            MeshRenderer.enabled = true;
        }
        else
        {
            active = false;
            MeshRenderer.enabled = false;
        }
    }

    public void Activate()
    {
        SetActive(true);
    }

    public void DeActivate()
    {
        SetActive(false);
    }
}
