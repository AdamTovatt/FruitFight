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

    public Texture2D GetImage()
    {
        RenderTexture.active = _camera.targetTexture;
        Texture2D renderResult = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height);
        renderResult.ReadPixels(rect, 0, 0);
        renderResult.Apply();
        return renderResult;
    }
}
