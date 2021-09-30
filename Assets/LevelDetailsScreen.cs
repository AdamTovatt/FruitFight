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

    public LoginScreen LoginScreen;

    private BrowseLevelsScreen parentScreen;

    private WorldMetadata currentWorldMetadata;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
    }

    public void Show(WorldMetadata worldMetadata, BrowseLevelsScreen parentScreen, bool localLevel)
    {
        currentWorldMetadata = worldMetadata;
        this.parentScreen = parentScreen;

        ThumbnailImage.sprite = worldMetadata.GetImageDataAsSprite();
        LevelTitleText.text = worldMetadata.Name;
        LevelDescriptionText.text = worldMetadata.Description;
        LevelAuthorText.text = string.Format("Created by: {0}", localLevel ? "you" : worldMetadata.AuthorName);

        if (localLevel) //this is a level from the file system
        {
            PublishOnlineButton.gameObject.SetActive(true);

            PublishOnlineButton.onClick.RemoveAllListeners();
            PublishOnlineButton.onClick.AddListener(PublishOnlineButtonWasClicked);

            LikeButton.gameObject.SetActive(false);
        }
        else //this is a level from the online library
        {
            PublishOnlineButton.gameObject.SetActive(false);
            LikeButton.gameObject.SetActive(true);
        }

        ButtonContainer.CenterContent();
        SelectDefaultButton();
    }

    public void SelectDefaultButton()
    {
        PlayButton.Select();
    }

    private async void PublishOnlineButtonWasClicked()
    {
        if (ApiHelper.UserCredentials == null)
        {
            LoginScreen.OnLoginScreenWasExited += LoginScreenWasClosed;
            LoginScreen.gameObject.SetActive(true);
            LoginScreen.Show(this);
        }
        else
        {
            World world = World.FromJson(FileHelper.LoadMapData(currentWorldMetadata.Name));
            world.Metadata = currentWorldMetadata;
            ErrorResponse uploadResult = await ApiLevelManager.UploadLevel(world);

            if (uploadResult == null)
                AlertCreator.Instance.CreateNotification("Level was uploaded!");
            else if (uploadResult.ErrorCode == "23505")
                AlertCreator.Instance.CreateNotification("You have already uploaded a level with this name");
            else
                AlertCreator.Instance.CreateNotification(uploadResult.Message);
        }
    }

    private void LoginScreenWasClosed()
    {
        if (ApiHelper.UserCredentials != null)
            AlertCreator.Instance.CreateNotification("Login successful");
        else
            AlertCreator.Instance.CreateNotification("You have not been logged in");
    }

    private void Close()
    {
        parentScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
        parentScreen.SelectDefaultButton();
    }
}
