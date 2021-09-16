using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelPropertiesScreen : MonoBehaviour
{
    public Button CloseButton;
    public Button CaptureThumbnailButton;
    public Image ThumbnailImage;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        CaptureThumbnailButton.onClick.AddListener(CaptureThumbnail);
    }

    private void CaptureThumbnail()
    {
        WorldEditor.Instance.OnImageWasCaptured += ThumbnailWasCaptured;
        WorldEditor.Instance.CaptureImage();
    }

    public void ThumbnailWasCaptured(Texture2D image)
    {
        ThumbnailImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
        //File.WriteAllText("c:\\users\\adam\\desktop\\screenshot.txt", Convert.ToBase64String(image.EncodeToJPG()));
    }

    public void Close()
    {
        WorldEditorUi.Instance.CloseLevelProperties();
    }

    public void Show()
    {
        TouchScreenKeyboard.Open("text", TouchScreenKeyboardType.Default);
    }
}
