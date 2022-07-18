using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEars : MonoBehaviour
{
    public AudioListener AudioListener;
    public UniqueSoundSourceManager SoundSourceManager;

    private SingleTargetCamera camera;

    private void Awake()
    {
        DisableSound();
    }

    private void Update()
    {
        if(camera != null)
        {
            transform.rotation = camera.transform.rotation;
        }
    }

    public void DisableSound()
    {
        AudioListener.enabled = false;
        SoundSourceManager.enabled = false;
    }

    public void EnableSound()
    {
        Debug.Log("Enable sound");
        AudioListener.enabled = true;
        SoundSourceManager.enabled = true;
    }

    public void SetCamera(SingleTargetCamera camera)
    {
        this.camera = camera;
    }
}
