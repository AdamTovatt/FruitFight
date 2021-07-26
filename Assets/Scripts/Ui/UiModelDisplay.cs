using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiModelDisplay : MonoBehaviour
{
    public static int UiModelIndex;

    public GameObject ModelPrefab;

    public GameObject Model { get; private set; }
    private RawImage rawImage;
    private UiModelCamera modelCamera;

    public delegate void OnImageGeneratedHandler(Texture2D image);
    public event OnImageGeneratedHandler OnImageGenerated;

    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        Model = Instantiate(ModelPrefab, Vector3.zero + Vector3.down * 1000 + Vector3.left * UiModelIndex++ * 100, Quaternion.identity);
        modelCamera = Model.GetComponentInChildren<UiModelCamera>();
        modelCamera.AssignDisplay(this);

        if (modelCamera.Texture != null)
            rawImage.texture = modelCamera.Texture;
    }

    public IEnumerator GenerateImage()
    {
        UiBananaMan uiBananaMan = Model.GetComponent<UiBananaMan>();
        if(uiBananaMan == null)
            throw new System.Exception("Only UiBananaMan can generate image as of right now");

        Quaternion originalRotation = uiBananaMan.Rotate.transform.rotation;
        uiBananaMan.Rotate.SetRotation(Quaternion.identity * Quaternion.Euler(0, 180, 0));

        yield return new WaitForEndOfFrame();

        OnImageGenerated?.Invoke(modelCamera.GetImage());
        OnImageGenerated = null;

        uiBananaMan.Rotate.SetRotation(originalRotation);
    }

    public void SetRawImageTexture(RenderTexture texture)
    {
        rawImage.texture = texture;
    }
}
