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

    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        Model = Instantiate(ModelPrefab, Vector3.zero + Vector3.down * 1000 + Vector3.left * UiModelIndex++ * 100, Quaternion.identity);
        modelCamera = Model.GetComponentInChildren<UiModelCamera>();
        modelCamera.AssignDisplay(this);

        if (modelCamera.Texture != null)
            rawImage.texture = modelCamera.Texture;
    }

    public void SetRawImageTexture(RenderTexture texture)
    {
        rawImage.texture = texture;
    }
}
