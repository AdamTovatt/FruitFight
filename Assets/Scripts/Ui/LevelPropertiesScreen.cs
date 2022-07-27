using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPropertiesScreen : MonoBehaviour
{
    public TMP_InputField LevelNameInput;
    public Button CloseButton;
    public Button CaptureThumbnailButton;
    public Button IncreaseTimeButton;
    public Button DecreaseTimeButton;
    public TextMeshProUGUI TimeText;
    public GameObject CaptureThumbnailCameraOverlay;
    public Image ThumbnailImage;
    public ImageTransition ThumbnailTransition;

    public GameObject FlashWhiteTransitionPrefab;

    private Sprite nullSprite;

    private void Awake()
    {
        nullSprite = ThumbnailImage.sprite;
    }

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        CaptureThumbnailButton.onClick.AddListener(CaptureThumbnail);
        LevelNameInput.onEndEdit.AddListener((text) => LevelNameWasSet(text));
        IncreaseTimeButton.onClick.AddListener(IncreaseTime);
        DecreaseTimeButton.onClick.AddListener(DecreaseTime);
    }

    private void CaptureThumbnail()
    {
        CaptureThumbnailCameraOverlay.gameObject.SetActive(true);
        WorldEditor.Instance.OnImageWasCaptured += ThumbnailWasCaptured;
        WorldEditor.Instance.CaptureImage();
    }

    private void IncreaseTime()
    {
        if (WorldEditor.Instance.CurrentWorld.TimeOfDay < 24)
        {
            WorldEditor.Instance.CurrentWorld.TimeOfDay += 0.25f;
            SetTimeOfDayText();
            SetTimeOfDayLight();
        }
    }

    private void DecreaseTime()
    {
        if (WorldEditor.Instance.CurrentWorld.TimeOfDay > 0)
        {
            WorldEditor.Instance.CurrentWorld.TimeOfDay -= 0.25f;
            SetTimeOfDayText();
            SetTimeOfDayLight();
        }
    }

    public void ThumbnailWasCaptured(Texture2D image)
    {
        CaptureThumbnailCameraOverlay.gameObject.SetActive(false);

        if (image != null)
        {
            AudioManager.Instance.Play("cameraShutter");
            Instantiate(FlashWhiteTransitionPrefab, transform);

            Sprite squareSprite = Sprite.Create(image, new Rect((image.width - image.height) / 2, 0, image.height, image.height), Vector2.zero);

            ThumbnailImage.sprite = nullSprite;

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
    }

    private void LevelNameWasSet(string text)
    {
        string trimmed = text.Trim();
        LevelNameInput.text = trimmed;

        if (!string.IsNullOrEmpty(trimmed))
            WorldEditor.Instance.CurrentWorld.Metadata.Name = trimmed;
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
            if (!string.IsNullOrEmpty(metadata.Name))
                LevelNameInput.text = metadata.Name;

            if (metadata.ImageData != null)
            {
                Debug.Log("Image data is not null");
                Texture2D thumbnail = metadata.GetImageDataAsTexture2d();
                ThumbnailImage.sprite = Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), Vector2.zero);
            }
            else
            {
                ThumbnailImage.sprite = nullSprite;
            }
        }
        else
        {
            WorldEditor.Instance.CurrentWorld.Metadata = new WorldMetadata();
        }

        SetTimeOfDayText();

        CloseButton.Select();
    }

    private void SetTimeOfDayText()
    {
        float timeOfDay = WorldEditor.Instance.CurrentWorld.TimeOfDay;
        float hours = Mathf.Floor(timeOfDay);
        float minutes = (timeOfDay - hours) * 60;

        TimeText.text = string.Format("{0}:{1}", hours.ToString("00"), minutes.ToString("00"));
    }

    private void SetTimeOfDayLight()
    {
        if (DaylightController.Instance != null)
        {
            DaylightController.Instance.Initialize(WorldEditor.Instance.CurrentWorld.NorthRotation, WorldEditor.Instance.CurrentWorld.TimeOfDay);
        }
    }
}
