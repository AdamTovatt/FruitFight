using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Button UpdateOnlineVersionButton;
    public Button RemovePublishedButton;

    public CenterContentContainer ButtonContainer;

    public LoginScreen LoginScreen;

    private BrowseLevelsScreen parentScreen;

    private WorldMetadata currentWorldMetadata;
    private bool currentLocalLevel;

    private GetLevelResponse currentGetLevelResponse;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
        PlayButton.onClick.AddListener(PlayLevel);
    }

    public void Show(WorldMetadata worldMetadata, BrowseLevelsScreen parentScreen, bool localLevel)
    {
        currentGetLevelResponse = null;
        currentLocalLevel = localLevel;
        currentWorldMetadata = worldMetadata;
        this.parentScreen = parentScreen;

        GetCurrentLevel();

        ThumbnailImage.sprite = worldMetadata.GetImageDataAsSprite();
        LevelTitleText.text = worldMetadata.Name;
        LevelDescriptionText.text = worldMetadata.Description;

        LevelAuthorText.text = string.Format("Created by: {0}", localLevel ? "you" : (ApiHelper.UserCredentials != null && ApiHelper.UserCredentials.UserId == worldMetadata.AuthorId ? worldMetadata.AuthorName + " (you)" : worldMetadata.AuthorName));

        if (localLevel) //this is a level from the file system
        {
            if (worldMetadata.Id == 0) //this level isn't published, at least as we know it
            {
                PublishOnlineButton.gameObject.SetActive(true);
                UpdateOnlineVersionButton.gameObject.SetActive(false);
                RemovePublishedButton.gameObject.SetActive(false);

                PublishOnlineButton.onClick.RemoveAllListeners();
                PublishOnlineButton.onClick.AddListener(PublishOnlineButtonWasClicked);
            }
            else //this level is published
            {
                PublishOnlineButton.gameObject.SetActive(false);
                UpdateOnlineVersionButton.gameObject.SetActive(true);
                RemovePublishedButton.gameObject.SetActive(true);

                RemovePublishedButton.onClick.RemoveAllListeners();
                RemovePublishedButton.onClick.AddListener(RemoveOnlineButtonWasClicked);
            }

            LikeButton.gameObject.SetActive(false);
        }
        else //this is a level from the online library
        {
            if (ApiHelper.UserCredentials != null && ApiHelper.UserCredentials.UserId == worldMetadata.AuthorId) //this is our level
            {
                RemovePublishedButton.gameObject.SetActive(true);
                UpdateOnlineVersionButton.gameObject.SetActive(true);
                LikeButton.gameObject.SetActive(false);

                RemovePublishedButton.onClick.RemoveAllListeners();
                RemovePublishedButton.onClick.AddListener(RemoveOnlineButtonWasClicked);
            }
            else //this is not our level
            {
                RemovePublishedButton.gameObject.SetActive(false);
                UpdateOnlineVersionButton.gameObject.SetActive(false);
                LikeButton.gameObject.SetActive(true);
            }

            PublishOnlineButton.gameObject.SetActive(false);
        }

        ButtonContainer.CenterContent();
        SelectDefaultButton();
    }

    private async void GetCurrentLevel()
    {
        currentGetLevelResponse = await ApiLevelManager.GetLevel(currentWorldMetadata.Id);
    }

    public void SelectDefaultButton()
    {
        PlayButton.Select();
    }

    private async void UpdateOnlineButtonWasClicked()
    {

    }

    private async void RemoveOnlineButtonWasClicked()
    {
        if (await ApiLevelManager.DeleteLevel(currentWorldMetadata.Id))
        {
            currentWorldMetadata.Id = 0;
            FileHelper.SaveMetadataToDisk(currentWorldMetadata);

            if (!currentLocalLevel)
            {
                Close();
                parentScreen.Show();
            }
            else
            {
                Show(currentWorldMetadata, parentScreen, currentLocalLevel);
            }

            AlertCreator.Instance.CreateNotification("Level was removed from online library");
        }
        else
        {
            AlertCreator.Instance.CreateNotification("Error when removing level");
        }
    }

    private void RedirectToLogin()
    {
        LoginScreen.OnLoginScreenWasExited += LoginScreenWasClosed;
        LoginScreen.gameObject.SetActive(true);
        LoginScreen.Show(this);
    }

    private async void PublishOnlineButtonWasClicked()
    {
        if (ApiHelper.UserCredentials == null || !ApiHelper.UserCredentials.Valid)
        {
            RedirectToLogin();
        }
        else
        {
            World world = World.FromJson(FileHelper.LoadMapData(currentWorldMetadata.Name));

            if (world.Metadata.Id != 0)
            {
                AlertCreator.Instance.CreateNotification("This level is already published");
                return;
            }

            world.Metadata = currentWorldMetadata;

            UploadLevelResponse uploadResult = await ApiLevelManager.UploadLevel(world);

            if (uploadResult.Success)
            {
                AssignIdToLocalFile(currentWorldMetadata, uploadResult.LevelId);
                AlertCreator.Instance.CreateNotification("Level was uploaded!");
            }
            else
            {
                if (uploadResult.ErrorResponse != null && uploadResult.ErrorResponse.ErrorCode == "23505")
                    AlertCreator.Instance.CreateNotification("You have already uploaded a level with this name");
                else if (uploadResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    RedirectToLogin();
                else
                    AlertCreator.Instance.CreateNotification(uploadResult.ErrorResponse == null ? "Unknown error" : uploadResult.ErrorResponse.Message);
            }
        }
    }

    private async void PlayLevel()
    {
        World world = null;

        if (currentLocalLevel)
        {
            world = World.FromJson(FileHelper.LoadMapData(currentWorldMetadata.Name));
        }
        else
        {
            if (currentGetLevelResponse == null)
            {
                currentGetLevelResponse = await ApiLevelManager.GetLevel(currentWorldMetadata.Id);
            }

            world = World.FromJson(currentGetLevelResponse.WorldData);
        }

        DontDestroyOnLoad(MainMenuUi.Instance.gameObject);
        MainMenuUi.Instance.MouseOverSelectableChecker.Disable();
        MainMenuUi.Instance.LoadingScreen.gameObject.SetActive(true);
        WorldBuilder.NextLevel = world;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadScene("GamePlay");
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MainMenuUi.Instance.LoadingScreen.gameObject.SetActive(false);
        MainMenuUi.Instance.gameObject.SetActive(false);
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void AssignIdToLocalFile(WorldMetadata worldMetadata, long id)
    {
        worldMetadata.Id = id;
        FileHelper.SaveMetadataToDisk(worldMetadata);
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
