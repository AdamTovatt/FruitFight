using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelDetailsScreen : MonoBehaviour
{
    public Image ThumbnailImage;
    public TextMeshProUGUI LevelTitleText;
    public TextMeshProUGUI LevelDescriptionText;
    public TextMeshProUGUI LevelAuthorText;

    public Button CloseButton;
    public Button PlayButton;
    public Button LikeButton;
    public Button PublishOnlineButton;

    public CenterContentContainer ButtonContainer;

    private BrowseLevelsScreen parentScreen;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
    }

    public void Show(WorldMetadata worldMetadata, BrowseLevelsScreen parentScreen, bool localLevel)
    {
        this.parentScreen = parentScreen;

        ThumbnailImage.sprite = worldMetadata.GetImageDataAsSprite();
        LevelTitleText.text = worldMetadata.Name;
        LevelDescriptionText.text = worldMetadata.Description;
        LevelAuthorText.text = string.Format("Created by: {0}", localLevel ? "you" : worldMetadata.AuthorName);

        if(localLevel) //this is a level from the file system
        {
            PublishOnlineButton.gameObject.SetActive(true);
            LikeButton.gameObject.SetActive(false);
        }
        else //this is a level from the online library
        {
            PublishOnlineButton.gameObject.SetActive(false);
            LikeButton.gameObject.SetActive(true);
        }

        ButtonContainer.CenterContent();
        PlayButton.Select();
    }

    private void Close()
    {
        parentScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
