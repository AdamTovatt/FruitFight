using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPropertiesScreen : MonoBehaviour
{
    public Button CloseButton;
    public Button CaptureThumbnailButton;
    public Button SetLevelNameButton;
    public TextMeshProUGUI LevelNameText;
    public GameObject CaptureThumbnailCameraOverlay;
    public Image ThumbnailImage;
    public ImageTransition ThumbnailTransition;

    public GameObject FlashWhiteTransitionPrefab;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        CaptureThumbnailButton.onClick.AddListener(CaptureThumbnail);
        SetLevelNameButton.onClick.AddListener(SetLevelName);
    }

    private void CaptureThumbnail()
    {
        CaptureThumbnailCameraOverlay.gameObject.SetActive(true);
        WorldEditor.Instance.OnImageWasCaptured += ThumbnailWasCaptured;
        WorldEditor.Instance.CaptureImage();
    }

    public void ThumbnailWasCaptured(Texture2D image)
    {
        AudioManager.Instance.Play("cameraShutter");
        Instantiate(FlashWhiteTransitionPrefab, transform);
        CaptureThumbnailCameraOverlay.gameObject.SetActive(false);

        Sprite squareSprite = Sprite.Create(image, new Rect((image.width - image.height) / 2, 0, image.height, image.height), Vector2.zero);

        ThumbnailImage.sprite = null;

        ThumbnailTransition.enabled = true;
        ThumbnailTransition.OnDidTransition += ThumbnailTransitionWasCompleted;
        ThumbnailTransition.SetImage(squareSprite);
        ThumbnailTransition.DoTransition(Vector2.zero, squareSprite.rect.size, ThumbnailImage.rectTransform.localPosition, ThumbnailImage.rectTransform.sizeDelta, 1f);

        Texture2D lowRes = new Texture2D(image.height, image.height);
        lowRes.SetPixels(squareSprite.texture.GetPixels((image.width - image.height) / 2, 0, image.height, image.height, 0));
        lowRes.Apply();
        lowRes = ResizeTexture2d(lowRes, 512, 512);

        WorldEditor.Instance.CurrentWorld.Metadata.ImageData = Convert.ToBase64String(lowRes.EncodeToJPG());
    }

    private void SetLevelName()
    {
        WorldEditorUi.Instance.OnScreenKeyboard.OnGotText += (sender, success, text) =>
        {
            if (success)
            {
                WorldEditor.Instance.CurrentWorld.Metadata.Name = text;
                LevelNameText.text = text;
            }

            SetLevelNameButton.Select();
        };

        WorldEditorUi.Instance.OnScreenKeyboard.OpenKeyboard();
    }

    private static Texture2D ResizeTexture2d(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }

    private void ThumbnailTransitionWasCompleted()
    {
        Texture2D thumbnail = WorldEditor.Instance.CurrentWorld.Metadata.GetImageDataAsTexture2d();
        ThumbnailImage.sprite = Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), Vector2.zero);
    }

    public void Close()
    {
        WorldEditorUi.Instance.CloseLevelProperties();
    }

    public void Show()
    {
        WorldMetadata metadata = WorldEditor.Instance.CurrentWorld.Metadata;

        if (metadata != null)
        {
            if (metadata.Name != null)
            {
                LevelNameText.text = metadata.Name;
            }
            else
            {
                LevelNameText.text = "(Untitled level)";
            }

            if (metadata.ImageData != null)
            {
                Debug.Log("Image data is not null");
                Texture2D thumbnail = metadata.GetImageDataAsTexture2d();
                ThumbnailImage.sprite = Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), Vector2.zero);
            }
            else
            {
                ThumbnailImage.sprite = null;
            }
        }
        else
        {
            WorldEditor.Instance.CurrentWorld.Metadata = new WorldMetadata();
        }

        SetLevelNameButton.Select();
    }
}
