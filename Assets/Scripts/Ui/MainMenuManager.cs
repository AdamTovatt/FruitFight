using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public Transform MainCameraSpawnPoint;
    public GameObject MainCameraPrefab;

    private SkyboxCamera skyboxCamera;
    private Transform mainCameraInstance;

    private void Start()
    {
        mainCameraInstance = Instantiate(MainCameraPrefab, MainCameraSpawnPoint.position, MainCameraSpawnPoint.rotation).transform;
        if(skyboxCamera != null)
        {
            skyboxCamera.SetMainCamera(mainCameraInstance);
        }
    }

    public void ActivateObjectWithDelay(GameObject theObject, float delay)
    {
        this.CallWithDelay(() => { theObject.SetActive(true); }, delay);
    }
}
