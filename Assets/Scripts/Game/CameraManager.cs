using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public GameObject CameraPrefab;

    public List<SingleTargetCamera> Cameras { get; set; } = new List<SingleTargetCamera>();

    public EventCamera CurrentEventCamera;

    private void Update()
    {
        if (CurrentEventCamera != null)
        {
            foreach (SingleTargetCamera camera in Cameras)
            {
                if (camera.ControlledByEventCamera)
                    camera.GetPositionFromEventCamera(CurrentEventCamera);
            }
        }
    }

    public SingleTargetCamera AddCamera(Transform target, PlayerInput input, bool allowInput = true)
    {
        SingleTargetCamera camera = Instantiate(CameraPrefab, target.position, Quaternion.identity).GetComponent<SingleTargetCamera>();
        camera.Initialize(target, input, allowInput);
        Cameras.Add(camera);

        return camera;
    }

    public void DisableCameraInput()
    {
        foreach (SingleTargetCamera camera in Cameras)
        {
            camera.AllowInput = false;
        }
    }

    public void EnableCameraInput()
    {
        foreach (SingleTargetCamera camera in Cameras)
        {
            camera.AllowInput = true;
        }
    }

    public void ShowEventCamera(EventCamera eventCamera)
    {
        CurrentEventCamera = eventCamera;
        foreach (SingleTargetCamera camera in Cameras)
        {
            camera.StartControlByEventCamera();
        }
    }

    public void StopShowingEventCamera()
    {
        CurrentEventCamera = null;
        foreach (SingleTargetCamera camera in Cameras)
        {
            camera.StopControlByEventCamera();
        }
    }
}
