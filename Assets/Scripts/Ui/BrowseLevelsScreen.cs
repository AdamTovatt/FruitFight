using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrowseLevelsScreen : MonoBehaviour
{
    public Button CloseButton;
    public Button ChangeViewModeButton;
    public RectTransform ButtonContainerSizeReference;
    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI ViewModeButtonText;

    public GameObject ButtonContainerPrefab;

    public LevelDetailsScreen LevelDetailsScreen;

    private LevelButtonContainer instantiatedButtonContainer;

    private int currentPage = 0;
    private int currentOffset = 0;
    private bool viewingLocalLevels;

    private void Start()
    {
        CloseButton.onClick.AddListener(MainMenuUi.Instance.ExitBrowseLevelsScreen);
        ChangeViewModeButton.onClick.AddListener(ViewModeButtonClicked);
    }

    public void SelectDefaultButton()
    {
        ChangeViewModeButton.Select();
    }

    public void Show()
    {
        SelectDefaultButton();

        ViewModeWasChanged(viewingLocalLevels);

        PopulateLevelList();
    }

    private void PopulateLevelList()
    {
        LoadingText.gameObject.SetActive(false);

        if (!viewingLocalLevels)
        {
            LoadingText.gameObject.SetActive(true);
            FetchLevels();
        }
        else
        {
            ShowLevels(FileHelper.LoadMetadatasFromDisk());
        }
    }

    private void ViewModeButtonClicked()
    {
        ViewModeWasChanged(!viewingLocalLevels);

        PopulateLevelList();
    }

    private void ViewModeWasChanged(bool newViewModeValue)
    {
        viewingLocalLevels = newViewModeValue;

        if (viewingLocalLevels)
        {
            TitleText.text = "Levels from the local library:";
            ViewModeButtonText.text = "Show online library";
        }
        else
        {
            TitleText.text = "Levels from the online library:";
            ViewModeButtonText.text = "Show local library";
        }
    }

    private async Task FetchLevels()
    {
        CleanLevelList();

        try
        {
            WorldMetadataResponse response = await ApiLevelManager.GetLevelsList(6, currentPage);

            if (response != null)
            {
                ShowLevels(response.Levels);
            }

            LoadingText.gameObject.SetActive(false);
        }
        catch(HttpRequestException)
        {
            LoadingText.gameObject.SetActive(true);
            Hover hover = LoadingText.gameObject.GetComponent<Hover>();
            hover.Stop();
            hover.enabled = false;
            LoadingText.text = "Network error";
            AlertCreator.Instance.CreateNotification("A connection to the server could not be established")[0].SetAlpha(255);
        }
    }

    private void LevelWasSelected(WorldMetadata metadata)
    {
        LevelDetailsScreen.gameObject.SetActive(true);
        LevelDetailsScreen.Show(metadata, this, viewingLocalLevels);
        gameObject.SetActive(false);
    }

    private async void ButtonContainerSwitchedPage(int newOffset)
    {
        if (currentOffset + newOffset < 0)
        {
            currentOffset = 0;
            currentPage = 0;
        }
        else
        {
            if (newOffset > 0)
                currentPage++;
            else if (newOffset < 0)
                currentPage--;

            currentOffset += newOffset;
        }

        await FetchLevels();

        if (newOffset > 0)
            instantiatedButtonContainer.NextPageButton.Select();
        else
            instantiatedButtonContainer.PreviousPageButton.Select();
    }

    private void CleanLevelList()
    {
        if (instantiatedButtonContainer != null)
        {
            Debug.Log("clean");
            instantiatedButtonContainer.OnPageSwitchRequested -= ButtonContainerSwitchedPage;
            instantiatedButtonContainer.Remove();
            Destroy(instantiatedButtonContainer.gameObject);
        }
    }

    public LevelButtonContainer ShowLevels(List<WorldMetadata> levels)
    {
        CleanLevelList();

        instantiatedButtonContainer = Instantiate(ButtonContainerPrefab, transform).GetComponent<LevelButtonContainer>();

        instantiatedButtonContainer.ClearLevelSelectSubscribersOnEventInvoke = false;

        if (!viewingLocalLevels)
            instantiatedButtonContainer.SetPaginationIsControlledExternally();

        if (!viewingLocalLevels)
            instantiatedButtonContainer.OnPageSwitchRequested += ButtonContainerSwitchedPage;

        instantiatedButtonContainer.SetSize(ButtonContainerSizeReference.sizeDelta.x, ButtonContainerSizeReference.sizeDelta.y);
        instantiatedButtonContainer.SetPosition(ButtonContainerSizeReference.localPosition.x, ButtonContainerSizeReference.localPosition.y);
        instantiatedButtonContainer.DisableBackgroundImage();

        instantiatedButtonContainer.Show(levels, false);

        if (!viewingLocalLevels)
            instantiatedButtonContainer.SetPageNumberText(currentPage, 10);

        instantiatedButtonContainer.OnLevelWasSelected += LevelWasSelected;

        return instantiatedButtonContainer;
    }
}
