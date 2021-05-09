using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiModelCamera : MonoBehaviour
{
    public RenderTexture renderTexturePrefab;
    public RenderTexture Texture { get; private set; }

    private Camera _camera;
    private UiModelDisplay modelDisplay;

    void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
        Texture = Instantiate(renderTexturePrefab);
        _camera.targetTexture = Texture;

        if (modelDisplay != null)
            modelDisplay.SetRawImageTexture(Texture);
    }

    public void AssignDisplay(UiModelDisplay display)
    {
        modelDisplay = display;
    }
}
