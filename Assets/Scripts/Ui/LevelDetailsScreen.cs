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
    public Button PlayButtonLocal;
    public Button PlayButtonNotOwner;
    public Button PlayButtonOwner;
    public Button OpenInEditorButton;
    public LoadingSpinnerButton UploadLevelOnlineButton;
    public LoadingSpinnerButton NegativeButtonNotOwner;
    public LoadingSpinnerButton NeutralButtonNotOwner;
    public LoadingSpinnerButton PositiveButtonNotOwner;
    public LoadingSpinnerButton NegativeButtonOwner;
    public LoadingSpinnerButton NeutralButtonOwner;
    public LoadingSpinnerButton PositiveButtonOwner;
    public LoadingSpinnerButton UpdateOnlineVersionButton;
    public LoadingSpinnerButton RemoveOnlineVersionButton;

    public GameObject LocalPanel;
    public GameObject NotOwnerPanel;
    public GameObject OwnerPanel;

    public CenterContentContainer ButtonContainer;

    public LoginScreen LoginScreen;

    public Color NeutralColor;
    public Color PositiveColor;
    public Color NegativeColor;

    private BrowseLevelsScreen parentScreen;

    private WorldMetadata currentWorldMetadata;
    private bool currentLocalLevel;

    private GetLevelResponse currentGetLevelResponse;

    private Selectable DefaultButton;
    private LoadingSpinnerButton PositiveRateButton;
    private LoadingSpinnerButton NeutralRateButton;
    private LoadingSpinnerButton NegativeRateButton;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
        PlayButtonLocal.onClick.RemoveAllListeners();
        PlayButtonNotOwner.onClick.RemoveAllListeners();
        PlayButtonOwner.onClick.RemoveAllListeners();
        OpenInEditorButton.onClick.RemoveAllListeners();
        UploadLevelOnlineButton.Button.onClick.RemoveAllListeners();
        NegativeButtonNotOwner.Button.onClick.RemoveAllListeners();
        NeutralButtonNotOwner.Button.onClick.RemoveAllListeners();
        PositiveButtonNotOwner.Button.onClick.RemoveAllListeners();
        NegativeButtonOwner.Button.onClick.RemoveAllListeners();
        NeutralButtonOwner.Button.onClick.RemoveAllListeners();
        PositiveButtonOwner.Button.onClick.RemoveAllListeners();
        UpdateOnlineVersionButton.Button.onClick.RemoveAllListeners();
        RemoveOnlineVersionButton.Button.onClick.RemoveAllListeners();
        CloseButton.onClick.RemoveAllListeners();
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

        LevelAuthorText.text = string.Format("Created by: {0}", localLevel ? "you" : (ApiHelper.UserCredentials != null && ApiHelper.UserCredentials.UserId == worldMetadata.AuthorId ? worldMetadata.AuthorName + " (you)" : worldMetadata.AuthorName));

        if (localLevel) //this is a level from the file system
        {
            if (worldMetadata.Id == 0) //this level isn't published, at least as we know it
            {
                ShowLocalLevel();
            }
            else //this level is published
            {
                ShowLocalLevel();
            }
        }
        else //this is a level from the online library
        {
            if (ApiHelper.UserCredentials != null && ApiHelper.UserCredentials.UserId == worldMetadata.AuthorId) //this is our level
            {
                ShowOwnerLevel();
            }
            else //this is not our level
            {
                ShowNonOwnerLevel();
            }
        }

        ButtonContainer.CenterContent();
        SelectDefaultButton();
    }

    private void ShowLocalLevel()
    {
        LocalPanel.SetActive(true);
        NotOwnerPanel.SetActive(false);
        OwnerPanel.SetActive(false);

        PlayButtonLocal.onClick.AddListener(PlayLevel);
    }

    private void ShowNonOwnerLevel()
    {
        PositiveRateButton = PositiveButtonNotOwner;
        NeutralRateButton = NeutralButtonNotOwner;
        NegativeRateButton = NegativeButtonNotOwner;

        PositiveButtonNotOwner.Button.onClick.AddListener(RatePositive);
        PlayButtonNotOwner.onClick.AddListener(PlayLevel);

        LocalPanel.SetActive(false);
        NotOwnerPanel.SetActive(true);
        OwnerPanel.SetActive(false);
    }

    private void ShowOwnerLevel()
    {
        PositiveRateButton = PositiveButtonOwner;
        NeutralRateButton = NeutralButtonOwner;
        NegativeRateButton = NegativeButtonOwner;

        PositiveButtonOwner.Button.onClick.AddListener(RatePositive);
        PlayButtonOwner.onClick.AddListener(PlayLevel);

        LocalPanel.SetActive(false);
        NotOwnerPanel.SetActive(false);
        OwnerPanel.SetActive(true);
    }

    private async void GetCurrentLevel()
    {
        currentGetLevelResponse = await ApiLevelManager.GetLevel(currentWorldMetadata.Id);
    }

    public void SelectDefaultButton()
    {
        if (DefaultButton != null)
            DefaultButton.Select();
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

    private async void RatePositive()
    {
        PositiveRateButton.ShowSpinner();
        bool success = await ApiLevelManager.LikeLevel(currentWorldMetadata.Id);
        PositiveRateButton.ReturnToNormal();
        PositiveRateButton.Button.image.color = success ? PositiveColor : NegativeColor;
    }

    private async void PlayLevel()
    {
        GameStateManager.SetGameState(GameState.Free);
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
            world.Metadata = currentGetLevelResponse.Metadata;

            ApiLevelManager.IncreasePlays(world.Metadata.Id);
        }

        if (!CustomNetworkManager.IsOnlineSession)
        {
            DontDestroyOnLoad(MainMenuUi.Instance.gameObject);
            MainMenuUi.Instance.MouseOverSelectableChecker.Disable();
            MainMenuUi.Instance.LoadingScreen.gameObject.SetActive(true);
            WorldBuilder.NextLevel = world;
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadScene("GamePlay");
        }
        else
        {
            if (world.Metadata.Id == 0)
            {
                AlertCreator.Instance.CreateNotification("Only levels from the online library can be played in online multiplayer");
            }
            else
            {
                NetworkMethodCaller.Instance.RpcClientShouldStartLevel(world.Metadata.Id);
            }
        }
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
