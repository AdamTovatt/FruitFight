using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public Transform MainCameraSpawnPoint;
    public GameObject MainCameraPrefab;

    private Spawner skyboxSpawner;
    private SkyboxCamera skyboxCamera;
    private Transform mainCameraInstance;

    private void Awake()
    {
        skyboxSpawner = gameObject.GetComponent<Spawner>();
        skyboxSpawner.OnObjectSpawned += (sender, spawnedObject) =>
        {
            skyboxCamera = spawnedObject.GetComponentInChildren<SkyboxCamera>();
            if (mainCameraInstance != null)
            {
                skyboxCamera.SetMainCamera(mainCameraInstance);
            }
        };
    }

    private void Start()
    {
        mainCameraInstance = Instantiate(MainCameraPrefab, MainCameraSpawnPoint.position, MainCameraSpawnPoint.rotation).transform;
        if(skyboxCamera != null)
        {
            skyboxCamera.SetMainCamera(mainCameraInstance);
        }
    }
}
